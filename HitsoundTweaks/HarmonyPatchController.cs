using System;
using HarmonyLib;
using Zenject;

namespace HitsoundTweaks;

public class HarmonyPatchController : IInitializable, IDisposable
{
    private readonly Harmony harmony = new("com.galaxymaster.hitsoundtweaks");

    public void Initialize()
    {
        harmony.PatchAll(Plugin.ExecutingAssembly);
    }

    public void Dispose()
    {
        harmony.UnpatchSelf();
    }
}