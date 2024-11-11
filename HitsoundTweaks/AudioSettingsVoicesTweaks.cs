using UnityEngine;

namespace HitsoundTweaks;

internal static class AudioSettingsVoicesTweaks
{
    public static void SetVoices(int numVirtualVoices, int numRealVoices)
    {
        Plugin.Log.Debug($"Attempting to set number of virtual voices to {numVirtualVoices}");
        Plugin.Log.Debug($"Attempting to set number of real voices to {numRealVoices}");

        AudioSettings.Reset(AudioSettings.GetConfiguration() with
        {
            numVirtualVoices = numVirtualVoices,
            numRealVoices = numRealVoices
        });     
    }
}