using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace HitsoundTweaks.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool IgnoreSaberSpeed { get; set; } = false;
        public virtual bool StaticSoundPos { get; set; } = false;
        public virtual bool EnableSpatialization { get; set; } = true;
        public virtual float RandomPitchMin { get; set; } = 0.9f;
        public virtual float RandomPitchMax { get; set; } = 1.2f;
        public virtual bool EnableChainElementHitsounds { get; set; } = false;
        public virtual float ChainElementVolumeMultiplier { get; set; } = 1.0f;
    }
}