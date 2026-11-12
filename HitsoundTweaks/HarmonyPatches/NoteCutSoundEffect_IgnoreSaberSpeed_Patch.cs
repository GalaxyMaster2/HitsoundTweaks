using HitsoundTweaks.Configuration;
using SiraUtil.Affinity;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * This enables configuration for ignoring saber speed
 */
internal class NoteCutSoundEffect_IgnoreSaberSpeed_Patch : IAffinity
{
    private readonly PluginConfig config;

    private NoteCutSoundEffect_IgnoreSaberSpeed_Patch(PluginConfig config)
    {
        this.config = config;
    }

    [AffinityPrefix]
    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.Init))]
    private void Prefix(ref bool ignoreSaberSpeed, AudioSource ____audioSource)
    {
        // if true, always play hitsounds even if saber isn't moving
        ignoreSaberSpeed = config.IgnoreSaberSpeed;
    }
}