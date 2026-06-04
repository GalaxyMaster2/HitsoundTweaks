using SiraUtil.Affinity;
using System;
using UnityEngine;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * For some reason, either the dspTime or the AudioSource time doesn't behave as it should, resulting in the _dspTimeOffset field oscillating between several discrete values
 * This causes hitsound timings to be irregular, which is audible to the player
 * To fix this, we patch out the _dspTimeOffset update, and reimplement it to follow a cumulative average of the target offset calculated each frame
 * The actual _dspTimeOffset value is set to whatever target offset we find that is closest to the average
 * Why? Because the target offset is always either exactly right, or off by a significant amount
 * The average tells us which of the encountered values should be considered the correct one
 * With this approach, sample-perfect timing is achieved reliably
 */
internal class AudioTimeSyncController_dspTimeOffset_Patch : IAffinity
{
    // reimplement _dspTimeOffset correction
    private bool firstCorrectionDone = false;
    private int averageCount = 1;
    private double averageOffset = 0.0;

    private double dspTimeOffset = 0;

    [AffinityPatch(typeof(AudioTimeSyncController), nameof(AudioTimeSyncController.Update))]
    private void Postfix(ref double ____dspTimeOffset, AudioSource ____audioSource, float ____timeScale, IAudioTimeSource.State ____state)
    {
        const double maxDiscrepancy = 0.05;

        // the cumulative average trends towards a consistent slightly desynced value, which this offset compensates for
        // this value works well at both 90 and 60 fps, so I'm assuming it's independent of framerate
        const double syncOffset = -0.0043;

        if (____state == IAudioTimeSource.State.Stopped)
        {
            firstCorrectionDone = false; // easiest way to reset this flag, Update is reliably called at least a few frames before playback starts
            dspTimeOffset = 0;
            return;
        }

        // Keep the original `dspTimeOffset` during recording
        if (Time.captureFramerate != 0) {
            return;
        }

        var audioTime = ____audioSource.timeSamples / (double)____audioSource.clip.frequency;
        var targetOffset = AudioSettings.dspTime - (audioTime / (double)____timeScale);

        if (!firstCorrectionDone || Math.Abs(averageOffset - targetOffset) > maxDiscrepancy)
        {
            averageOffset = targetOffset;
            averageCount = 1;
            firstCorrectionDone = true;
            dspTimeOffset = targetOffset;
        }
        else
        {
            // lock in value after some time
            if (averageCount < 10000)
            {
                // update cumulative average
                averageOffset = (averageOffset * averageCount + targetOffset) / (averageCount + 1);
                averageCount++;

                // set dspTimeOffset to whatever targetOffset encountered that is closest to the average
                if (Math.Abs(targetOffset - (averageOffset + syncOffset)) < Math.Abs(dspTimeOffset - (averageOffset + syncOffset)))
                {
                    dspTimeOffset = targetOffset;
                }
            }
        }

        ____dspTimeOffset = dspTimeOffset;
    }
}
