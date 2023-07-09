using HarmonyLib;
using HitsoundTweaks.Configuration;

namespace HitsoundTweaks.HarmonyPatches
{
    /*
     * By default, hitsounds are not played for chain elements
     * These patches enable them and apply a volume multiplier based on config values
     */
    [HarmonyPatch(typeof(NoteCutSoundEffectManager), "IsSupportedNote")]
    internal class NoteCutSoundEffectManager_Chain_Element_Hitsound_Patch
    {
        static void Postfix(NoteData noteData, ref bool __result)
        {
            if (noteData.gameplayType == NoteData.GameplayType.BurstSliderElement && noteData.colorType != ColorType.None)
            {
                __result = PluginConfig.Instance.EnableChainElementHitsounds;
            }
        }
    }

    [HarmonyPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.Init))]
    internal class NoteCutSoundEffect_Chain_Element_Volume_Multiplier_Patch
    {
        static void Prefix(ref float volumeMultiplier, NoteController noteController)
        {
            // apply chain element volume multiplier
            if (noteController.noteData.gameplayType == NoteData.GameplayType.BurstSliderElement)
            {
                volumeMultiplier *= PluginConfig.Instance.ChainElementVolumeMultiplier;
            }
        }
    }
}
