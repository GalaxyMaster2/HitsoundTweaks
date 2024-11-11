using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * Chain elements and heads can be spawned out of order, which can cause hitsounds to be skipped if chain element hitsounds are enabled
 * By making the time proximity check use an absolute value, we mitigate this issue
 */
[HarmonyPatch(typeof(NoteCutSoundEffectManager), nameof(NoteCutSoundEffectManager.HandleNoteWasSpawned))]
internal class NoteCutSoundEffectManager_Proximity_Check_Patch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        // disable note time proximity check
        for (int i = 0; i < code.Count - 21; i++)
        {
            if (code[i + 7].opcode == OpCodes.Ldc_R4 && (float)code[i + 7].operand == 0.001f)
            {
                // 22 NOP instructions wheeeeeeeeee
                for (int j = 0; j < 22; j++)
                {
                    code[i + j].opcode = OpCodes.Nop;
                }
                break;
            }
        }

        // disable volume reduction for same time notes of different color
        for (int i = 0; i < code.Count - 41; i++)
        {
            if (code[i + 24].opcode == OpCodes.Callvirt && (MethodInfo)code[i + 24].operand == AccessTools.PropertySetter(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.volumeMultiplier)))
            {
                // haha NOP go brr (I'm sorry)
                for (int j = 0; j < 42; j++)
                {
                    code[i + j].opcode = OpCodes.Nop;
                }
                break;
            }
        }

        return code;
    }

    static bool Prefix(NoteController noteController, float ____prevNoteATime, float ____prevNoteBTime, NoteCutSoundEffect ____prevNoteASoundEffect, NoteCutSoundEffect ____prevNoteBSoundEffect, float ____beatAlignOffset, out bool __state)
    {
        __state = false; // whether or not we should multiply volume in the Postfix
        NoteData noteData = noteController.noteData;

        // reimplement note time proximity check by absolute value, to allow for out of order spawned notes (needed for chains)
        if ((noteData.colorType == ColorType.ColorA && Mathf.Abs(noteData.time - ____prevNoteATime) < 0.001f) || (noteData.colorType == ColorType.ColorB && Mathf.Abs(noteData.time - ____prevNoteBTime) < 0.001f))
        {
            return false;
        }

        // reimplement volume reduction for same time notes of different color to account for out of order notes
        var reduceVolume = false;
        if (Mathf.Abs(noteData.time - ____prevNoteATime) < 0.001f || Mathf.Abs(noteData.time - ____prevNoteBTime) < 0.001f)
        {
            // add extra null check just to be safe, though it shouldn't be necessary
            if (noteData.colorType == ColorType.ColorA && ____prevNoteBSoundEffect != null && ____prevNoteBSoundEffect.enabled)
            {
                reduceVolume = true;

                // base game does = but *= makes more sense, especially with chain hitsounds
                ____prevNoteBSoundEffect.volumeMultiplier *= 0.9f;
            }
            else if (noteData.colorType == ColorType.ColorB && ____prevNoteASoundEffect != null && ____prevNoteASoundEffect.enabled)
            {
                reduceVolume = true;
                ____prevNoteASoundEffect.volumeMultiplier *= 0.9f;
            }
        }

        // if flag2 is true, then the volume was already multiplied by the original method
        var flag2 = noteData.timeToPrevColorNote < ____beatAlignOffset;
        __state = reduceVolume && !flag2;

        return true;
    }

    // multiply volume if indicated by Prefix
    static void Postfix(NoteController noteController, NoteCutSoundEffect ____prevNoteASoundEffect, NoteCutSoundEffect ____prevNoteBSoundEffect, bool __state, bool __runOriginal)
    {
        if (__state && __runOriginal)
        {
            var noteColor = noteController.noteData.colorType;

            // the previous sound effect has now been assigned to the one just created, so we can access it here to adjust volume
            if (noteColor == ColorType.ColorA)
            {
                ____prevNoteASoundEffect.volumeMultiplier *= 0.9f;
            }
            else if (noteColor == ColorType.ColorB)
            {
                ____prevNoteBSoundEffect.volumeMultiplier *= 0.9f;
            }
        }
    }
}