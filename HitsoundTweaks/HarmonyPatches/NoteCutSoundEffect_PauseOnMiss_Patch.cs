using HitsoundTweaks.Configuration;
using SiraUtil.Affinity;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * For original hitsounds: always schedule playback, but pause the hitsound if the note is missed.
 * Unpause for any slice (good or bad cut). Repause if the note is missed.
 * This preserves the game's scheduling logic and works for vanilla hitsounds.
 *
 */
internal class NoteCutSoundEffect_PauseOnMiss_Patch : IAffinity
{
    private readonly PluginConfig config;
    private NoteCutSoundEffect_PauseOnMiss_Patch(PluginConfig config)
    {
        this.config = config;
    }

    public static bool isNoteShort;
    [AffinityPatch(typeof(NoteCutSoundEffectManager), nameof(NoteCutSoundEffectManager.HandleNoteWasSpawned))]
    private void Postfix(NoteController noteController, float ____beatAlignOffset)
    {
        // Determine if the note is short for use in muting logic, this is needed to prevent spatialization artifacts on short notes.
        NoteData noteData = noteController.noteData;
        if (config.PauseHitSoundWhenMissed)
        {
            isNoteShort = noteData.timeToPrevColorNote < ____beatAlignOffset;
        }
    }

    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.Init))]
    private void Postfix(Saber saber, AudioSource ____audioSource, NoteCutSoundEffect __instance, bool ____noteWasCut, Saber ____saber)
    {
        if (config.PauseHitSoundWhenMissed)
        {
            // Mute the sound if the saber is not moving fast enough to ensure the woosh sound only plays when swinging. Or if the note is short.
            if (!____noteWasCut)
            {
                bool flag = ____saber.bladeSpeed > 15f && !isNoteShort;
                ____audioSource.mute = !flag;
            }
        }
    }

    [AffinityPatch(typeof(NoteCutSoundEffect), nameof(NoteCutSoundEffect.OnLateUpdate))]
    private void Postfix(bool ____noteWasCut, AudioSource ____audioSource, double ____startDSPTime, float ____aheadTime, Saber ____saber)
    {
        if (____audioSource == null || !config.PauseHitSoundWhenMissed)
            return;


        // If the scheduled note time has passed and the note was not cut, pause the hitsound.
        if (config.PauseHitSoundWhenMissed) 
        {
            // compute the note's DSP time (the time the note is considered "hit" by the audio scheduling)
            double noteDSPTime = ____startDSPTime + (double)____aheadTime;
            double pauseOffset = -0.02;
            double pauseSpatializedOffset = -0.04; // 4 we give an earlier offset to mute the sound to prevent spatialization artifacts.
            bool spatializerPresent = SpatializerDetectionHelper.spatializerPresent;
            double offsetToUse = ____audioSource.spatialize & spatializerPresent ? pauseSpatializedOffset : pauseOffset;
            if (!____noteWasCut && AudioSettings.dspTime >= noteDSPTime + offsetToUse)
            {
                ____audioSource.Pause();
            }
            else if (____noteWasCut)
            {
                ____audioSource.mute = false;
                ____audioSource.UnPause();
            }

        }


    }

}

