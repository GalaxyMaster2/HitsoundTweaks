using System;
using HarmonyLib;
using Zenject;

namespace HitsoundTweaks;

public class HarmonyPatchController : IInitializable, IDisposable
{
    private Harmony _harmony;

    private HarmonyPatchController()
    {
        _harmony = new Harmony("com.galaxymaster.hitsoundtweaks");
    }
    
    public void Initialize()
    {
        _harmony.PatchAll(Plugin.ExecutingAssembly);
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }
}