using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace HitsoundTweaks.HarmonyPatches
{
    public class SpatializerDetectionHelper : IInitializable
    {
        public static bool spatializerPresent;
        // See if a spatializer plugin is present through AudioSettings GetSpatializerPluginName method
        public void Initialize()
        {
            DetectSpatializerPlugin();
        }
        public static string DetectSpatializerPlugin()
        {
            string name = GetSpatializerPluginNameSafe();
            if (!string.IsNullOrEmpty(name))
            {
                Plugin.Log.Info($"Spatializer reported by Unity: {name}");
                spatializerPresent = true;
                return name;
            }
            spatializerPresent = false;
            Plugin.Log.Warn("No spatializer plugin detected! Downgrade below or at 1.40.2 for a spatializer.");
            return null;
        }

        private static string GetSpatializerPluginNameSafe()
        {
            try
            {
                var mi = typeof(AudioSettings).GetMethod("GetSpatializerPluginName", BindingFlags.Public | BindingFlags.Static);
                if (mi != null)
                {
                    return mi.Invoke(null, null) as string;
                }
            }
            catch
            {
                // API unavailable or reflection failed
            }
            return null;
        }

    }
}
