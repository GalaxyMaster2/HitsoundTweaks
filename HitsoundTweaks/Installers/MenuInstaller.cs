using HitsoundTweaks.UI;
using Zenject;

namespace HitsoundTweaks.Installers;

public class MenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.Bind<SettingsMenu>().AsSingle();
        Container.BindInterfacesTo<SettingsMenuManager>().AsSingle();
    }
}