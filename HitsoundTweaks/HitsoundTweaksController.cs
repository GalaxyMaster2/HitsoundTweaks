using HarmonyLib;
using UnityEngine;
using BeatSaberMarkupLanguage.Settings;
using HitsoundTweaks.Configuration;

namespace HitsoundTweaks
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class HitsoundTweaksController : MonoBehaviour
    {
        public static HitsoundTweaksController Instance { get; private set; }

        public int NumVirtualVoices = 32;

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

            var config = AudioSettings.GetConfiguration();
            config.numVirtualVoices = 128;
            config.numRealVoices = 64;
            Plugin.Log?.Info($"Attempting to set number of virtual voices to {config.numVirtualVoices}");
            Plugin.Log?.Info($"Attempting to set number of real voices to {config.numRealVoices}");
            AudioSettings.Reset(config);
            var newConfig = AudioSettings.GetConfiguration();
            NumVirtualVoices = newConfig.numVirtualVoices;

            if (newConfig.numVirtualVoices == config.numVirtualVoices)
            {
                Plugin.Log?.Info($"Successfully set number of virtual voices");
            }
            else
            {
                Plugin.Log?.Warn($"Failed to set number of virtual voices to the desired value");
                Plugin.Log?.Info($"Number of virtual voices is currently: {NumVirtualVoices}");
            }

            if (newConfig.numRealVoices == config.numRealVoices)
            {
                Plugin.Log?.Info($"Successfully set number of real voices");
            }
            else
            {
                Plugin.Log?.Warn($"Failed to set number of real voices to the desired value");
                Plugin.Log?.Info($"Number of real voices is currently: {newConfig.numRealVoices}");
            }

            var harmony = new Harmony("com.galaxymaster.hitsoundtweaks");
            harmony.PatchAll();

            BSMLSettings.instance.AddSettingsMenu("HitsoundTweaks", "HitsoundTweaks.UI.ModSettingsView.bsml", PluginConfig.Instance);

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
    }
}
