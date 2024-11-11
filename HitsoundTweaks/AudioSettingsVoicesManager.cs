using UnityEngine;
using Zenject;

namespace HitsoundTweaks;

public class AudioSettingsVoicesManager : IInitializable
{
    private const int NumVirtualVoices = 128;
    private const int NumRealVoices = 64;
    
    public int CurrentNumVirtualVoices { get; private set; } = AudioSettings.GetConfiguration().numVirtualVoices;
    
    public void Initialize()
    {
        AudioSettingsVoicesTweaks.SetVoices(NumVirtualVoices, NumRealVoices);
        var newConfig = AudioSettings.GetConfiguration();

        CurrentNumVirtualVoices = newConfig.numVirtualVoices;

        if (newConfig.numVirtualVoices == NumVirtualVoices)
        {
            Plugin.Log.Info($"Successfully set number of virtual voices");
        }
        else
        {
            Plugin.Log.Warn($"Failed to set number of virtual voices to the desired value");
            Plugin.Log.Info($"Number of virtual voices is currently: {CurrentNumVirtualVoices}");
        }

        if (newConfig.numRealVoices == NumRealVoices)
        {
            Plugin.Log.Info($"Successfully set number of real voices");
        }
        else
        {
            Plugin.Log.Warn($"Failed to set number of real voices to the desired value");
            Plugin.Log.Info($"Number of real voices is currently: {newConfig.numRealVoices}");
        }
    }
}