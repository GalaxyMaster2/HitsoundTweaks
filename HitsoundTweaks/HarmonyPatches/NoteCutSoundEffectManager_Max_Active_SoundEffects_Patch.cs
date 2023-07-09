using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches
{
    /*
     * The game has a built in check to end playing hitsounds early if the number of playing sounds exceeds a certain value
     * By default, this value is higher than the number of virtual voices, making it effectively do nothing
     * This is likely intentional to keep hitsounds from cutting out entirely with the limited number of virtual voices available by default
     * When the number of virtual voices is raised, it makes more sense to re-enable this check by setting the limit just below the number of virtual voices
     */
    [HarmonyPatch(typeof(NoteCutSoundEffectManager), "HandleNoteWasSpawned")]
    internal class NoteCutSoundEffectManager_Max_Active_SoundEffects_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            // set limit of number of NoteCutSoundEffects currently active
            for (int i = 0; i < code.Count - 1; i++)
            {
                if (code[i].opcode == OpCodes.Callvirt && (MethodInfo)code[i].operand == AccessTools.PropertyGetter(typeof(List<NoteCutSoundEffect>), "Count"))
                {
                    // subtract 8 to give some overhead, don't go below base game value of 64
                    code[i + 1].operand = Mathf.Max(HitsoundTweaksController.CurrentNumVirtualVoices - 8, 64);
                    break;
                }
            }
            return code;
        }
    }
}
