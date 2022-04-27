# HitsoundTweaks
Beat Saber mod that adds more configurability to hitsounds, and fixes several base game bugs to make them more reliable and consistent.

# Usage
By default, this mod does nothing (other than fixing some bugs). Upon first launch, `UserData/HitsoundTweaks.json` will be created. There you can configure the various options this mod gives you.

# Configuration
All configuration is done in the `UserData/HitsoundTweaks.json` file. There is no UI to change the configuration in game, however changes made to the file will apply in real time while the game is running.

The available options are:
* `IgnoreSaberSpeed` (default `false`): When set to `true`, hitsounds always play even when the sabers are not moving. This is useful to hear hitsounds while in fpfc.
* `StaticSoundPos` (default `false`): By default, the game attaches the position of hitsounds to your saber tips. When this option is set to `true`, hitsounds are instead located at your feet. This can help make hitsound timings more consistent, and you may prefer how this sounds. When hitsound spatialization is disabled, this does nothing.
* `EnableSpatialization` (default `true`): Controls whether or not hitsounds are spatialized. Spatialization can make hitsounds sound quite different in game compared to outside it. Setting this to `false` makes hitsounds sound exactly as they do outside of the game.
* `RandomPitchMin` (default `0.9`): Sets the minimum pitch for hitsound pitch randomization.
* `RandomPitchMax` (default `1.2`): Sets the maximum pitch for hitsound pitch randomization. When min and max are equal, pitch is constant.
* `EnableChainElementHitsounds` (default `false`): By default, the game disables hitsounds for chain elements. With this option you can enable them.
* `ChainElementVolumeMultiplier` (default `1.0`): Allows you to scale the volume of chain element hitsounds independent of other hitsounds. Primarily useful to lower this a bit if you find chain hitsounds to be overly loud.
