using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * This makes 2 changes to make hitsounds play more reliably
 * 1. Scheduling playback silently fails when there are no virtual voices available. To alleviate this, we retry scheduling playback just ahead of time.
 * 2. AudioSource priorities are not managed very smoothly, which gives Unity little time to sort things out, leading to hitsounds cutting out, not playing, etc.
 *    This reworks priority assignment to smoothly scale priorities based on time relative to the note it's meant for, which greatly improves reliability
 */
[HarmonyPatch(typeof(NoteCutSoundEffect), "OnLateUpdate")]
internal class Hitsound_Reliability_Patches
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        // disable setting audio source priority
        for (int i = 0; i < code.Count - 3; i++)
        {
            if (code[i + 3].opcode == OpCodes.Callvirt && (MethodInfo)code[i + 3].operand == AccessTools.PropertySetter(typeof(AudioSource), nameof(AudioSource.priority)))
            {
                // don't want to deal with fixing branches, so just replace it with NOPs
                for (int j = 0; j < 4; j++)
                {
                    code[i + j].opcode = OpCodes.Nop;
                }
                break;
            }
        }

        return code;
    }

    // retry scheduling playback just ahead of time
    static void Prefix(AudioSource ____audioSource, double ____startDSPTime, ref bool ____isPlaying)
    {
        const double retryAheadTime = 0.05;
        if (!____isPlaying && ____startDSPTime - AudioSettings.dspTime < retryAheadTime)
        {
            // this is extremely hacky, absolutely terrible, and I shouldn't be doing this
            // but I have to keep track of state somewhere, and this seems to be unused
            // I'm sorry
            ____isPlaying = true;

            if (!____audioSource.isPlaying)
            {
                ____audioSource.Stop();
                ____audioSource.timeSamples = 0;
                ____audioSource.PlayScheduled(____startDSPTime);
            }
        }
    }

    static void Postfix(bool ____noteWasCut, AudioSource ____audioSource, bool ____goodCut, double ____startDSPTime, double ____aheadTime)
    {
        // set priority based on time relative to note time
        var dspTime = AudioSettings.dspTime;
        var noteTime = ____startDSPTime + ____aheadTime;
        if (!(____noteWasCut && !____goodCut)) // note hasn't been bad cut
        {
            if (dspTime - noteTime > 0)
            {
                const float priorityFalloff = 384;
                ____audioSource.priority = Mathf.Clamp(32 + Mathf.RoundToInt((float)(dspTime - noteTime) * priorityFalloff), 32, 127);
            }
            else
            {
                const float priorityRampup = 192;
                ____audioSource.priority = Mathf.Clamp(32 + Mathf.RoundToInt((float)(noteTime - dspTime) * priorityRampup), 32, 128);
            }
        }
    }
}