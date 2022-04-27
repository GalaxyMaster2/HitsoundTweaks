using HarmonyLib;
using HitsoundTweaks.Configuration;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches
{
    /*
     * This enables configuration for ignoring saber speed, and enabling/disabling hitsound spatialization
     */
    [HarmonyPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.Init))]
    internal class NoteCutSoundEffect_Misc_Patches
    {
        static void Prefix(ref bool ignoreSaberSpeed, AudioSource ____audioSource)
        {
            // if true, always play hitsounds even if saber isn't moving
            ignoreSaberSpeed = PluginConfig.Instance.IgnoreSaberSpeed;

            // enable/disable spatialization
            ____audioSource.spatialize = PluginConfig.Instance.EnableSpatialization;
        }
    }
}
