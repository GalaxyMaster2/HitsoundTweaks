using HarmonyLib;
using HitsoundTweaks.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches
{
    /*
     * This patch allows you to configure the range used for the randomized pitch of hitsounds
     * Also fixes a bug in the game where the sfxLatency is incorrectly scaled by pitch
     */
    [HarmonyPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.Init))]
    internal class NoteCutSoundEffect_Random_Pitch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            // remove randomized pitch assignment
            for (int i = 0; i < code.Count - 4; i++)
            {
                if (code[i + 4].opcode == OpCodes.Stfld && (FieldInfo)code[i + 4].operand == AccessTools.Field(typeof(NoteCutSoundEffect), "_pitch"))
                {
                    // don't want to deal with fixing branches, so just replace it with NOPs
                    for (int j = 0; j < 5; j++)
                    {
                        code[i + j].opcode = OpCodes.Nop;
                    }
                    break;
                }
            }

            return code;
        }

        static void Prefix(ref float aheadTime, ref float ____pitch)
        {
            // generate random pitch in the desired range
            var randomPitch = Random.Range(PluginConfig.Instance.RandomPitchMin, PluginConfig.Instance.RandomPitchMax);

            // fix broken pitch correction for hitsound alignment
            // - the game scales both the alignment offset and the sfx latency combined, while it should only scale the alignment offset
            const float hitsoundAlignOffset = 0.185f;
            var sfxLatency = PluginConfig.Instance.EnableSpatialization ? aheadTime - hitsoundAlignOffset : 0f;
            ____pitch = randomPitch;
            aheadTime = (hitsoundAlignOffset / randomPitch + sfxLatency) * randomPitch;
        }
    }
}
