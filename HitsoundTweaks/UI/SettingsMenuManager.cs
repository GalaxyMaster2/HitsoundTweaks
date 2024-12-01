using BeatSaberMarkupLanguage.Settings;
using System;
using Zenject;

namespace HitsoundTweaks.UI;

public class SettingsMenuManager : IInitializable, IDisposable
{
    private readonly BSMLSettings bsmlSettings;
    private readonly SettingsMenu settingsMenu;

    private const string SettingsMenuName = "HitsoundTweaks";
    private const string ResourcePath = "HitsoundTweaks.UI.ModSettingsView.bsml";

    private SettingsMenuManager(BSMLSettings bsmlSettings, SettingsMenu settingsMenu)
    {
        this.bsmlSettings = bsmlSettings;
        this.settingsMenu = settingsMenu;
    }

    public void Initialize()
    {
        bsmlSettings.AddSettingsMenu(SettingsMenuName, ResourcePath, settingsMenu);
    }

    public void Dispose()
    {
        bsmlSettings.RemoveSettingsMenu(SettingsMenuName);
    }
}