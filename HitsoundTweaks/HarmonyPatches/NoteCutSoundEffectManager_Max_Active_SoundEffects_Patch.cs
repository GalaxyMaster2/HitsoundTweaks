using HarmonyLib;
using SiraUtil.Affinity;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * The game has a built in check to end playing hitsounds early if the number of playing sounds exceeds a certain value
 * By default, this value is higher than the number of virtual voices, making it effectively do nothing
 * When the number of virtual voices is raised, this check becomes relevant again, and the threshold needs to be changed
 */
internal class NoteCutSoundEffectManager_Max_Active_SoundEffects_Patch : IAffinity
{
    private readonly AudioSettingsVoicesManager audioSettingsVoicesManager;

    private NoteCutSoundEffectManager_Max_Active_SoundEffects_Patch(AudioSettingsVoicesManager audioSettingsVoicesManager)
    {
        this.audioSettingsVoicesManager = audioSettingsVoicesManager;
    }

    [AffinityTranspiler]
    [AffinityPatch(typeof(NoteCutSoundEffectManager), nameof(NoteCutSoundEffectManager.HandleNoteWasSpawned))]
    private IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        for (int i = 0; i < code.Count - 1; i++)
        {
            if (code[i].opcode == OpCodes.Callvirt && (MethodInfo)code[i].operand == AccessTools.PropertyGetter(typeof(List<NoteCutSoundEffect>), "Count"))
            {
                // I cannot actually set the value to >127 because it's only 8 bit integer
                // so set it to 0 and flip the comparison instead to disable the check
                code[i + 1].operand = 0;
                code[i + 2].opcode = OpCodes.Bge_S;
                break;
            }
        }
        return code;
    }
}