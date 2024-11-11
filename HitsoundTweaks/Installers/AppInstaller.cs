using HitsoundTweaks.Configuration;
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
        
        Container.BindInterfacesTo<HarmonyPatchController>().AsSingle();
    }
}