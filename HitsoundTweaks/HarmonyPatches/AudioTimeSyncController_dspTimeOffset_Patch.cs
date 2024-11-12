using HarmonyLib;
using SiraUtil.Affinity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * For some reason, either the dspTime or the AudioSource time doesn't behave as it should, resulting in the _dspTimeOffset field oscillating between several discrete values
 * This causes hitsound timings to be irregular, which is audible to the player
 * To fix this, we patch out the _dspTimeOffset update, and reimplement it to follow a cumulative average of the target offset calculated each frame
 * The actual _dspTimeOffset value is set to whatever target offset we find that is closest to the average
 * Why? Because the target offset is always either exactly right, or off by a significant amount
 * The average tells us which of the encountered values should be considered the correct one
 * With this approach, sample-perfect timing is achieved reliably
 */
internal class AudioTimeSyncController_dspTimeOffset_Patch : IAffinity
{
    [AffinityTranspiler]
    [AffinityPatch(typeof(AudioTimeSyncController), nameof(AudioTimeSyncController.Update))]
    private IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
    private bool firstCorrectionDone = false;
    private int averageCount = 1;
    private double averageOffset = 0.0;

    [AffinityPatch(typeof(AudioTimeSyncController), nameof(AudioTimeSyncController.Update))]
    private void Postfix(ref double ____dspTimeOffset, AudioSource ____audioSource, float ____timeScale, AudioTimeSyncController.State ____state)
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
            ____dspTimeOffset = targetOffset;
        }
        else
        {
            // lock in value after some time
            if (averageCount < 10000)
            {
                // update cumulative average
                averageOffset = (averageOffset * averageCount + targetOffset) / (averageCount + 1);
                averageCount++;

                // set dspTimeOffset to whatever targetOffset encountered that is closest to the average
                if (Math.Abs(targetOffset - (averageOffset + syncOffset)) < Math.Abs(____dspTimeOffset - (averageOffset + syncOffset)))
                {
                    ____dspTimeOffset = targetOffset;
                }
            }
        }
    }
}
