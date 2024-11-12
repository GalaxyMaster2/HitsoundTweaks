using HitsoundTweaks.UI;
using Zenject;

namespace HitsoundTweaks.Installers;

public class MenuInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<SettingsMenuManager>().AsSingle();
    }
}