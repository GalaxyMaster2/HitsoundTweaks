using HitsoundTweaks.Configuration;
using HitsoundTweaks.HarmonyPatches;
using Zenject;

namespace HitsoundTweaks.Installers;

public class AppInstaller : Installer
{
    private readonly PluginConfig config;

    private AppInstaller(PluginConfig config)
    {
        this.config = config;
    }

    public override void InstallBindings()
    {
        Container.BindInstance(config).AsSingle();

        Container.BindInterfacesAndSelfTo<AudioSettingsVoicesManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<SpatializerDetectionHelper>().AsSingle();
        Container.BindInterfacesTo<AudioTimeSyncController_dspTimeOffset_Patch>().AsSingle();
        Container.BindInterfacesTo<Hitsound_Reliability_Patches>().AsSingle();
        Container.BindInterfacesTo<NoteCutSoundEffectManager_Proximity_Check_Patch>().AsSingle();
        Container.BindInterfacesTo<Early_Note_Spawn_Fix>().AsSingle();
    }
}
