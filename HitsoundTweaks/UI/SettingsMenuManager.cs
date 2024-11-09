using System;
using BeatSaberMarkupLanguage.Settings;
using HitsoundTweaks.Configuration;
using Zenject;

namespace HitsoundTweaks.UI;

public class SettingsMenuManager : IInitializable, IDisposable
{
    private readonly BSMLSettings _bsmlSettings;
    private readonly PluginConfig _config;
    
    private const string SettingsMenuName = "HitsoundTweaks";
    private const string ResourcePath = "HitsoundTweaks.UI.ModSettingsView.bsml";
    
    private SettingsMenuManager(BSMLSettings bsmlSettings, PluginConfig config)
    {
        _bsmlSettings = bsmlSettings;
        _config = config;
    }
    
    public void Initialize()
    {
        _bsmlSettings.AddSettingsMenu(SettingsMenuName, ResourcePath, _config);
    }

    public void Dispose()
    {
        _bsmlSettings.RemoveSettingsMenu(SettingsMenuName);
    }
}