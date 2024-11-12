using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * For some reason, either the dspTime or the AudioSource time doesn't behave as it should, resulting in the _dspTimeOffset field oscillating between 2 values
 * This causes hitsound timings to be irregular, which is audible to the player
 * To fix this, we patch out the _dspTimeOffset update, and reimplement it to be a cumulative average of the target offset calculated each frame
 * This reliably and consistently gets within a handful of audio samples after a few seconds, which is for all intents and purposes good enough
 */
[HarmonyPatch(typeof(AudioTimeSyncController), nameof(AudioTimeSyncController.Update))]
internal class AudioTimeSyncController_dspTimeOffset_Patch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        // remove _dspTimeOffset update
        var skip = true;
        for (int i = 0; i < code.Count - 8; i++)
        {
            if (code[i + 8].opcode == OpCodes.Stfld && (FieldInfo)code[i + 8].operand == AccessTools.Field(typeof(AudioTimeSyncController), "_dspTimeOffset"))
            {
                // we want the second time this instruction occurs
                if (skip)
                {
                    skip = false;
                    continue;
                }

                // don't want to deal with fixing branches, so just replace it with NOPs
                for (int j = 0; j < 9; j++)
                {
                    code[i + j].opcode = OpCodes.Nop;
                }
                break;
            }
        }
        return code;
    }

    // reimplement _dspTimeOffset correction
    static bool firstCorrectionDone = false;
    static int averageCount = 1;
    static double averageOffset = 0.0;
    static void Postfix(ref double ____dspTimeOffset, AudioSource ____audioSource, float ____timeScale, AudioTimeSyncController.State ____state)
    {
        const double maxDiscrepancy = 0.05;

        // the cumulative average trends towards a consistent slightly desynced value, which this offset compensates for
        // this value works well at both 90 and 60 fps, so I'm assuming it's independent of framerate
        const double syncOffset = -0.0043;

        if (____state == AudioTimeSyncController.State.Stopped)
        {
            firstCorrectionDone = false; // easiest way to reset this flag, Update is reliably called at least a few frames before playback starts
            return;
        }

        var audioTime = ____audioSource.timeSamples / (double)____audioSource.clip.frequency;
        var targetOffset = AudioSettings.dspTime - (audioTime / (double)____timeScale);

        if (!firstCorrectionDone || Math.Abs(averageOffset - targetOffset) > maxDiscrepancy)
        {
            averageOffset = targetOffset;
            averageCount = 1;
            firstCorrectionDone = true;
        }
        else
        {
            // lock in value after some time
            if (averageCount < 10000)
            {
                // update cumulative average
                averageOffset = (averageOffset * averageCount + targetOffset) / (averageCount + 1);
                averageCount++;
            }
        }

        ____dspTimeOffset = averageOffset + syncOffset;
    }
}
