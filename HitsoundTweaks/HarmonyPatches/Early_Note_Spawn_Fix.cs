using SiraUtil.Affinity;
using System.Collections.Generic;

namespace HitsoundTweaks.HarmonyPatches;

/*
 * When notes are spawned immediately upon map load, the ATSC has not started yet and as a result the dspTimeOffset will be wrong
 * This means hitsounds will be scheduled to a time in the past, effectively skipping them
 * To solve this, check if ATSC state is Playing before creating hitsounds, and if not, queue them for later processing
 */
internal class Early_Note_Spawn_Fix : IAffinity
{
    private readonly List<NoteController> initQueue = new();

    [AffinityPrefix]
    [AffinityPatch(typeof(NoteCutSoundEffectManager), nameof(NoteCutSoundEffectManager.HandleNoteWasSpawned))]
    private bool NoteSpawnPrefix(NoteController noteController, AudioTimeSyncController ____audioTimeSyncController)
    {
        if (____audioTimeSyncController.state != AudioTimeSyncController.State.Playing)
        {
            initQueue.Add(noteController);
            return false;
        }
        return true;
    }

    [AffinityPrefix]
    [AffinityPatch(typeof(NoteCutSoundEffectManager), nameof(NoteCutSoundEffectManager.LateUpdate))]
    private void LateUpdatePrefix(AudioTimeSyncController ____audioTimeSyncController, NoteCutSoundEffectManager __instance)
    {
        if (____audioTimeSyncController.state == AudioTimeSyncController.State.Playing && initQueue.Count > 0)
        {
            foreach (var item in initQueue)
            {
                __instance.HandleNoteWasSpawned(item);
            }
            initQueue.Clear();
        }
    }

}
