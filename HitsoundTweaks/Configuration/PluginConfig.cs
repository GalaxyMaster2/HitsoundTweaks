using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace HitsoundTweaks.Configuration;

internal class PluginConfig
{
    [UIValue("ignore-saber-speed")]
    public virtual bool IgnoreSaberSpeed { get; set; } = false;

    [UIValue("PauseHitSoundWhenMissed")]
    public virtual bool PauseHitSoundWhenMissed { get; set; } = false;

    [UIValue("static-sound-pos")]
    public virtual bool StaticSoundPos { get; set; } = false;

    [UIValue("enable-spatialization")]
    public virtual bool EnableSpatialization { get; set; } = true;

    [UIValue("followAfterCut")]
    public virtual bool followAfterCut { get; set; } = false;

    [UIValue("random-pitch-min")]
    public virtual float RandomPitchMin { get; set; } = 0.9f;

    [UIValue("random-pitch-max")]
    public virtual float RandomPitchMax { get; set; } = 1.2f;

    [UIValue("enable-chain-element-hitsounds")]
    public virtual bool EnableChainElementHitsounds { get; set; } = false;

    [UIValue("chain-element-volume-multiplier")]
    public virtual float ChainElementVolumeMultiplier { get; set; } = 0.8f;
}
