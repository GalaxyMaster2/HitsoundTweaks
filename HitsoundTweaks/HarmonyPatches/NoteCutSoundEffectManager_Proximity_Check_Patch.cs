using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches
{
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
                    // 21 NOP instructions wheeeeeeeeee
                    for (int j = 0; j < 22; j++)
                    {
                        code[i + j].opcode = OpCodes.Nop;
                    }
                    break;
                }
            }
            return code;
        }

        // reimplement note time proximity check by absolute value, to allow for out of order spawned notes (needed for chains)
        static bool Prefix(NoteController noteController, float ____prevNoteATime, float ____prevNoteBTime)
        {
            NoteData noteData = noteController.noteData;
            if ((noteData.colorType == ColorType.ColorA && Mathf.Abs(noteData.time - ____prevNoteATime) < 0.001f) || (noteData.colorType == ColorType.ColorB && Mathf.Abs(noteData.time - ____prevNoteBTime) < 0.001f))
            {
                return false;
            }

            return true;
        }
    }
}
