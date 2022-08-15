# HitsoundTweaks
Beat Saber mod that adds more configurability to hitsounds, and fixes several base game bugs to make them more reliable and consistent.

# Usage
By default, this mod does nothing (other than fixing some bugs). You can configure the various options this mod gives you in the Mod Settings menu.

# Configuration
Configuration is done in the Mod Settings menu in game, or in the configuration file at `UserData/HitsoundTweaks.json`. Changes made to the configuration file will apply in real time while the game is running.

The available options are:
* **Ignore Saber Speed** (`IgnoreSaberSpeed`, default `false`): When enabled, hitsounds always play even when the sabers are not moving. This is useful to hear hitsounds while in fpfc.
* **Static Sound Position** (`StaticSoundPos`, default `false`): When enabled, hitsounds are located at your feet, instead of your saber tips. This can help make hitsound timings more consistent, and you may prefer how this sounds. When hitsound spatialization is disabled, this does nothing.
* **Hitsound Spatialization** (`EnableSpatialization`, default `true`): When disabled, hitsounds will be played as-is without further processing, instead of being spatialized. Spatialization places sounds in 3D space and uses processing to fool your ears into hearing the sound at that location. This can make hitsounds sound quite different in game compared to outside it. Disabling it is primarily useful to play hitsounds exactly as they sound outside the game.
* **Random Pitch (min)** (`RandomPitchMin`, default `0.9`): The minimum value for randomized hitsound pitch.
* **Random Pitch (max)** (`RandomPitchMax`, default `1.2`): The maximum value for randomized hitsound pitch. If minimum and maximum are equal, hitsound pitch is not randomized.
* **Enable Chain Element Hitsounds** (`EnableChainElementHitsounds`, default `false`): When enabled, hitsounds will be played for chain elements.
* **Chain Volume Multiplier** (`ChainElementVolumeMultiplier`, default `0.8`): Volume multiplier used for chain element hitsounds. Primarily useful for lowering volume if you find chain hitsounds to be overly loud.
