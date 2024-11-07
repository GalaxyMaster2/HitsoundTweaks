using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.Util;
using HarmonyLib;
using HitsoundTweaks.Configuration;
using UnityEngine;

namespace HitsoundTweaks
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class HitsoundTweaksController : MonoBehaviour
    {
        public static HitsoundTweaksController Instance { get; private set; }

        private const int NumVirtualVoices = 128;
        private const int NumRealVoices = 64;
        
        public static int CurrentNumVirtualVoices { get; private set; } = 32;

        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            if (Instance != null)
            {
                Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            Instance = this;

            var harmony = new Harmony("com.galaxymaster.hitsoundtweaks");
            harmony.PatchAll();

            MainMenuAwaiter.MainMenuInitializing += OnMainMenuInitialized;
            
            Plugin.Log?.Debug($"{name}: Awake()");
        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            if (Instance == this)
                Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

        }
        #endregion
        
        private static void OnMainMenuInitialized()
        {
            BSMLSettings.Instance.AddSettingsMenu("HitsoundTweaks", "HitsoundTweaks.UI.ModSettingsView.bsml", PluginConfig.Instance);
            
            SetVoices(NumVirtualVoices, NumRealVoices);
        }

        private static void SetVoices(int numVirtualVoices, int numRealVoices)
        {
            var config = AudioSettings.GetConfiguration();
            config.numVirtualVoices = numVirtualVoices;
            config.numRealVoices = numRealVoices;
            Plugin.Log?.Info($"Attempting to set number of virtual voices to {numVirtualVoices}");
            Plugin.Log?.Info($"Attempting to set number of real voices to {numRealVoices}");
            AudioSettings.Reset(config);             
            var newConfig = AudioSettings.GetConfiguration();
            CurrentNumVirtualVoices = newConfig.numVirtualVoices;

            if (newConfig.numVirtualVoices == numVirtualVoices)
            {
                Plugin.Log?.Info($"Successfully set number of virtual voices");
            }
            else
            {
                Plugin.Log?.Warn($"Failed to set number of virtual voices to the desired value");
                Plugin.Log?.Info($"Number of virtual voices is currently: {CurrentNumVirtualVoices}");
            }

            if (newConfig.numRealVoices == numRealVoices)
            {
                Plugin.Log?.Info($"Successfully set number of real voices");
            }
            else
            {
                Plugin.Log?.Warn($"Failed to set number of real voices to the desired value");
                Plugin.Log?.Info($"Number of real voices is currently: {newConfig.numRealVoices}");
            }
        }
    }
}
