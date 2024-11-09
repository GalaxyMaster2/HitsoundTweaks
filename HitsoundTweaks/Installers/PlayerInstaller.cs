using HitsoundTweaks.HarmonyPatches;
using Zenject;

namespace HitsoundTweaks.Installers;

public class PlayerInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<NoteCutSoundEffectManager_Max_Active_SoundEffects_Patch>().AsSingle();
    }
}