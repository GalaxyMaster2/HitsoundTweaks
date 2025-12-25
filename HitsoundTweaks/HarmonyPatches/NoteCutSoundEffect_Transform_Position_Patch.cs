using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using HitsoundTweaks.Configuration;
using SiraUtil.Affinity;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * These patches allow the user to configure whether or not the spatialized hitsound should follow the sabers
 * If disabled, the hitsound will always play at the player's feet
 */
internal static class NoteCutSoundEffectExtensions
{
    // Store fade data for each NoteCutSoundEffect instance
    // Using a table to separate data from the original class
    private static readonly ConditionalWeakTable<NoteCutSoundEffect, FadeData> fadeTable = new();

    public class FadeData
    {
        public Vector3 startPos;
        public double startTime;
        public float duration;
        public bool active;
    }

    public static FadeData GetFadeData(this NoteCutSoundEffect instance)
        => fadeTable.GetOrCreateValue(instance);
}
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
        ____noteController, bool ____goodCut, NoteController noteController, in NoteCutInfo noteCutInfo, Saber ____saber)
    {
        if (____noteController != noteController)
        {
            return;
        }
        if (!config.StaticSoundPos && ____audioSource.spatialize)
        {

            // Apply the transformed position
            __instance.transform.position = noteCutInfo.cutPoint;
            if (config.followAfterCut)
            {
                var fade = __instance.GetFadeData();
                fade.startPos = noteCutInfo.cutPoint;
                fade.startTime = AudioSettings.dspTime;
                fade.duration = (0.434f / ____audioSource.pitch); // the amount of time in seconds it takes for the sound to fully follow the saber.
                fade.active = true;
            }

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
    private void Postfix(NoteCutSoundEffect __instance, AudioSource ____audioSource, Saber ____saber, bool ____noteWasCut)
    {
        if (!____noteWasCut && !config.StaticSoundPos && ____audioSource.spatialize)
        {
            __instance.transform.position = ____saber.saberBladeTopPosForLogic;
        }
        if (____audioSource.spatialize && !config.StaticSoundPos && config.followAfterCut)
        {
            // Get live saber midpoint positioning.
            Vector3 liveMidSaberPos = (____saber.saberBladeTopPosForLogic + ____saber.saberBladeBottomPosForLogic) / 2;
            // If fading, lerp from cutPoint to saber position over time
            var fade = __instance.GetFadeData();
            if (fade.active)
            {
                double elapsed = AudioSettings.dspTime - fade.startTime;
                float t = Mathf.Clamp01((float)elapsed / fade.duration);
                t = Mathf.SmoothStep(0f, 1f, t);

                // Start near cutPoint, end up tracking the live saber position
                Vector3 lerpedPos = Vector3.Lerp(fade.startPos, liveMidSaberPos, t);
                __instance.transform.position = lerpedPos;

                // Once fade fully completes, permanently follow saber
                if (t >= 1f)
                {
                    fade.active = false;
                }
            }
            else if (!fade.active)
            {
                // Continue following saber after fade completes
                __instance.transform.position = liveMidSaberPos;
            }
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
            __instance.transform.position = saber.saberBladeTopPosForLogic;
        }
    }
}
