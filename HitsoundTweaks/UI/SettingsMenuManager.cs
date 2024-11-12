using BeatSaberMarkupLanguage.Settings;
using HitsoundTweaks.Configuration;
using System;
using Zenject;

namespace HitsoundTweaks.UI;

public class SettingsMenuManager : IInitializable, IDisposable
{
    private readonly BSMLSettings bsmlSettings;
    private readonly PluginConfig config;

    private const string SettingsMenuName = "HitsoundTweaks";
    private const string ResourcePath = "HitsoundTweaks.UI.ModSettingsView.bsml";

    private SettingsMenuManager(BSMLSettings bsmlSettings, PluginConfig config)
    {
        this.bsmlSettings = bsmlSettings;
        this.config = config;
    }

    public void Initialize()
    {
        bsmlSettings.AddSettingsMenu(SettingsMenuName, ResourcePath, config);
    }

    public void Dispose()
    {
        bsmlSettings.RemoveSettingsMenu(SettingsMenuName);
    }
}