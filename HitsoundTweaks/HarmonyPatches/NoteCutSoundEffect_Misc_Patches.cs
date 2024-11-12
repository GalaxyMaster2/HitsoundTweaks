using HitsoundTweaks.Configuration;
using SiraUtil.Affinity;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * This enables configuration for ignoring saber speed, and enabling/disabling hitsound spatialization
 */
internal class NoteCutSoundEffect_Misc_Patches : IAffinity
{
    private readonly PluginConfig config;

    private NoteCutSoundEffect_Misc_Patches(PluginConfig config)
    {
        this.config = config;
    }
        
    [AffinityPrefix]
    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.Init))]
    private void Prefix(ref bool ignoreSaberSpeed, AudioSource ____audioSource)
    {
        // if true, always play hitsounds even if saber isn't moving
        ignoreSaberSpeed = config.IgnoreSaberSpeed;

        // enable/disable spatialization
        ____audioSource.spatialize = config.EnableSpatialization;
    }
}