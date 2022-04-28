using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches
{
    /*
     * For some reason, either the dspTime or the AudioSource time doesn't behave as it should, resulting in the _dspTimeOffset field oscillating between 2 values
     * This causes hitsound timings to be irregular, which is audible to the player
     * To fix this, we patch out the _dspTimeOffset update, and reimplement it to smoothly interpolate between the current and the target values
     * As far as I know, AudioSource time and dspTime should not diverge, but to account for any possibility of it happening, the correction is there
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
        static void Postfix(ref double ____dspTimeOffset, AudioSource ____audioSource, float ____timeScale, AudioTimeSyncController.State ____state)
        {
            const double maxDiscrepancy = 0.05;
            const float correctionRate = 0.0f; // smooth correction disabled as this seems to be causing drift somehow

            if (____state == AudioTimeSyncController.State.Stopped)
            {
                firstCorrectionDone = false; // easiest way to reset this flag, Update is reliably called at least a few frames before playback starts
                return;
            }

            var audioTime = ____audioSource.timeSamples / (double)____audioSource.clip.frequency;
            var targetOffset = AudioSettings.dspTime - (audioTime / ____timeScale);

            if (!firstCorrectionDone || Math.Abs(____dspTimeOffset - targetOffset) > maxDiscrepancy)
            {
                ____dspTimeOffset = targetOffset;
                firstCorrectionDone = true;
                return;
            }

            // Mathf.Lerp() doesn't work with doubles
            ____dspTimeOffset += (targetOffset - ____dspTimeOffset) * Mathf.Clamp01(Time.deltaTime * ____timeScale * correctionRate);
        }
    }
}
