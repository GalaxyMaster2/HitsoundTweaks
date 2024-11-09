using HitsoundTweaks.HarmonyPatches;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace HitsoundTweaks
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        public static int CurrentNumVirtualVoices { get; set; } = AudioSettings.GetConfiguration().numVirtualVoices;

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            
            zenjector.Install(Location.App, container =>
            {
                container.BindInterfacesTo<AudioSettingsVoicesManager>().AsSingle();
            });
            
            zenjector.Install(Location.Player, container =>
            {
                container.BindInterfacesTo<NoteCutSoundEffectManager_Max_Active_SoundEffects_Patch>().AsSingle();
            });
            
            AudioSettings.OnAudioConfigurationChanged += AudioSettingsOnOnAudioConfigurationChanged;
        }
        
        private void AudioSettingsOnOnAudioConfigurationChanged(bool devicewaschanged)
        {
            Log.Notice("Audio settings changed!");
            var config = AudioSettings.GetConfiguration();
            Log.Notice($"{config.numVirtualVoices} {config.numRealVoices} {config.sampleRate} {devicewaschanged}");
        }

        #region BSIPA Config
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            new GameObject("HitsoundTweaksController").AddComponent<HitsoundTweaksController>();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
        }
    }
}
