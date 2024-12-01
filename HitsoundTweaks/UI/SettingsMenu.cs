using BeatSaberMarkupLanguage.Attributes;
using HitsoundTweaks.Configuration;

namespace HitsoundTweaks.UI;

internal class SettingsMenu
{
    private readonly PluginConfig config;

    public SettingsMenu(PluginConfig config)
    {
        this.config = config;
    }

    [UIValue("ignore-saber-speed")]
    public bool IgnoreSaberSpeed
    {
        get => config.IgnoreSaberSpeed;
        set => config.IgnoreSaberSpeed = value;
    }

    [UIValue("static-sound-pos")]
    public bool StaticSoundPos
    {
        get => config.StaticSoundPos;
        set => config.StaticSoundPos = value;
    }

    [UIValue("enable-spatialization")]
    public bool EnableSpatialization
    {
        get => config.EnableSpatialization;
        set => config.EnableSpatialization = value;
    }

    [UIValue("random-pitch-min")]
    public float RandomPitchMin
    {
        get => config.RandomPitchMin;
        set => config.RandomPitchMin = value;
    }

    [UIValue("random-pitch-max")]
    public float RandomPitchMax
    {
        get => config.RandomPitchMax;
        set => config.RandomPitchMax = value;
    }

    [UIValue("enable-chain-element-hitsounds")]
    public bool EnableChainElementHitsounds
    {
        get => config.EnableChainElementHitsounds;
        set => config.EnableChainElementHitsounds = value;
    }

    [UIValue("chain-element-volume-multiplier")]
    public float ChainElementVolumeMultiplier
    {
        get => config.ChainElementVolumeMultiplier;
        set => config.ChainElementVolumeMultiplier = value;
    }
}