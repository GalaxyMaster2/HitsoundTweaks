using HitsoundTweaks.Configuration;
using SiraUtil.Affinity;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * By default, hitsounds are not played for chain elements
 * These patches enable them and apply a volume multiplier based on config values
 */
internal class NoteCutSoundEffectManager_Chain_Element_Hitsound_Patch : IAffinity
{
    private readonly PluginConfig config;

    private NoteCutSoundEffectManager_Chain_Element_Hitsound_Patch(PluginConfig config)
    {
        this.config = config;
    }

    [AffinityPatch(typeof(NoteCutSoundEffectManager), nameof(NoteCutSoundEffectManager.IsSupportedNote))]
    private void Postfix(NoteData noteData, ref bool __result)
    {
        if (noteData.gameplayType == NoteData.GameplayType.BurstSliderElement && noteData.colorType != ColorType.None)
        {
            __result = config.EnableChainElementHitsounds;
        }
    }
}

internal class NoteCutSoundEffect_Chain_Element_Volume_Multiplier_Patch : IAffinity
{
    private readonly PluginConfig config;

    private NoteCutSoundEffect_Chain_Element_Volume_Multiplier_Patch(PluginConfig config)
    {
        this.config = config;
    }

    [AffinityPrefix]
    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.Init))]
    private void Prefix(ref float volumeMultiplier, NoteController noteController)
    {
        // apply chain element volume multiplier
        if (noteController.noteData.gameplayType == NoteData.GameplayType.BurstSliderElement)
        {
            volumeMultiplier *= config.ChainElementVolumeMultiplier;
        }
    }
}