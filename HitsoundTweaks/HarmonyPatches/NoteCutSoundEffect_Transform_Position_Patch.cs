﻿using HarmonyLib;
using HitsoundTweaks.Configuration;
using SiraUtil.Affinity;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * These patches allow the user to configure whether or not the spatialized hitsound should follow the sabers
 * If disabled, the hitsound will always play at the player's feet
 */
internal class NoteCutSoundEffect_Transform_Position_NoteWasCut_Patch : IAffinity
{
    private readonly PluginConfig config;

    private NoteCutSoundEffect_Transform_Position_NoteWasCut_Patch(PluginConfig config)
    {
        this.config = config;
    }

    [AffinityTranspiler]
    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.NoteWasCut))]
    private IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        // disable transform position being set to cut point
        for (int i = 0; i < code.Count - 4; i++)
        {
            if (code[i + 4].opcode == OpCodes.Callvirt && (MethodInfo)code[i + 4].operand == AccessTools.PropertySetter(typeof(Transform), nameof(Transform.position)))
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

    // set transform position if desired
    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.NoteWasCut))]
    private void Postfix(NoteCutSoundEffect __instance, AudioSource ____audioSource, NoteController
        ____noteController, bool ____goodCut, NoteController noteController, in NoteCutInfo noteCutInfo)
    {
        if (____noteController != noteController)
        {
            return;
        }

        if (!____goodCut)
        {
            ____audioSource.spatialize = true;
            __instance.transform.position = noteCutInfo.cutPoint;
            return;
        }

        if (!config.StaticSoundPos && ____audioSource.spatialize)
        {
            __instance.transform.position = noteCutInfo.cutPoint;
        }
    }
}

internal class NoteCutSoundEffect_Transform_Position_LateUpdate_Patch : IAffinity
{
    private readonly PluginConfig config;

    private NoteCutSoundEffect_Transform_Position_LateUpdate_Patch(PluginConfig config)
    {
        this.config = config;
    }

    [AffinityTranspiler]
    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.OnLateUpdate))]
    private IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        // disable transform position being set to saber tip pos
        for (int i = 0; i < code.Count - 5; i++)
        {
            if (code[i + 5].opcode == OpCodes.Callvirt && (MethodInfo)code[i + 5].operand == AccessTools.PropertySetter(typeof(Transform), nameof(Transform.position)))
            {
                // don't want to deal with fixing branches, so just replace it with NOPs
                for (int j = 0; j < 6; j++)
                {
                    code[i + j].opcode = OpCodes.Nop;
                }
                break;
            }
        }

        return code;
    }

    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.OnLateUpdate))]
    private void Postfix(bool ____noteWasCut, Saber ____saber, AudioSource ____audioSource, NoteCutSoundEffect
        __instance)
    {
        // set transform position if desired
        if (!____noteWasCut && !config.StaticSoundPos && ____audioSource.spatialize)
        {
            __instance.transform.position = ____saber.saberBladeTopPos;
        }
    }
}

internal class NoteCutSoundEffect_Transform_Position_Init_Patch : IAffinity
{
    private readonly PluginConfig config;

    public NoteCutSoundEffect_Transform_Position_Init_Patch(PluginConfig config)
    {
        this.config = config;
    }

    [AffinityTranspiler]
    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.Init))]
    private IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        // set transform position to (0,0,0)
        for (int i = 0; i < code.Count - 2; i++)
        {
            if (code[i + 2].opcode == OpCodes.Callvirt && (MethodInfo)code[i + 2].operand == AccessTools.PropertySetter(typeof(Transform), nameof(Transform.position)))
            {
                code[i] = new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Vector3), nameof(Vector3.zero)));
                code.RemoveAt(i + 1);
                break;
            }
        }

        return code;
    }

    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.Init))]
    private void Postfix(Saber saber, AudioSource ____audioSource, NoteCutSoundEffect __instance)
    {
        // not sure how necessary this is, but since the game does it I might as well too
        if (!config.StaticSoundPos && ____audioSource.spatialize)
        {
            __instance.transform.position = saber.saberBladeTopPos;
        }
    }
}