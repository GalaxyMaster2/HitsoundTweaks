using HitsoundTweaks.HarmonyPatches;
using Zenject;

namespace HitsoundTweaks.Installers;

public class PlayerInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<NoteCutSoundEffect_Misc_Patches>().AsSingle();
        Container.BindInterfacesTo<NoteCutSoundEffect_Random_Pitch_Patch>().AsSingle();
        Container.BindInterfacesTo<NoteCutSoundEffect_Transform_Position_Init_Patch>().AsSingle();
        Container.BindInterfacesTo<NoteCutSoundEffect_Transform_Position_NoteWasCut_Patch>().AsSingle();
        Container.BindInterfacesTo<NoteCutSoundEffect_Transform_Position_LateUpdate_Patch>().AsSingle();
        Container.BindInterfacesTo<NoteCutSoundEffectManager_Chain_Element_Hitsound_Patch>().AsSingle();
        Container.BindInterfacesTo<NoteCutSoundEffectManager_Max_Active_SoundEffects_Patch>().AsSingle();
        Container.BindInterfacesTo<NoteCutSoundEffect_Chain_Element_Volume_Multiplier_Patch>().AsSingle();
        Container.BindInterfacesTo<NoteCutSoundEffect_PauseOnMiss_Patch>().AsSingle();
    }
}
