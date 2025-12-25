using System;
using BeatSaberMarkupLanguage.Settings;
using HitsoundTweaks.Configuration;
using HitsoundTweaks.HarmonyPatches;
using Zenject;

namespace HitsoundTweaks.UI;

public class SettingsMenuManager : IInitializable, IDisposable
{
    private readonly BSMLSettings bsmlSettings;
    private readonly PluginConfig config;

    private const string SettingsMenuName = "HitsoundTweaks";
    private const string ResourcePathNormal = "HitsoundTweaks.UI.ModSettingsView.bsml";
    private const string ResourcePathNoSpatial = "HitsoundTweaks.UI.ModSettingsView_NoSpatializer.bsml";

    private SettingsMenuManager(BSMLSettings bsmlSettings, PluginConfig config)
    {
        this.bsmlSettings = bsmlSettings;
        this.config = config;
    }

    public void Initialize()
    {
        // Detect spatializer once at startup and choose which BSML to register
        string resource = ResourcePathNormal;
        try
        {
            var detected = SpatializerDetectionHelper.DetectSpatializerPlugin();
            if (string.IsNullOrEmpty(detected))
                resource = ResourcePathNoSpatial;
        }
        catch
        {
            // On any detection error, fall back to the no-spatializer view
            resource = ResourcePathNoSpatial;
        }

        bsmlSettings.AddSettingsMenu(SettingsMenuName, resource, config);
    }

    public void Dispose()
    {
        bsmlSettings.RemoveSettingsMenu(SettingsMenuName);
    }
}
