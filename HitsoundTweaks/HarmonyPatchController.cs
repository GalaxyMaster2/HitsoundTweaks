using System;
using HarmonyLib;
using HitsoundTweaks.Configuration;
using JetBrains.Annotations;
using Zenject;

namespace HitsoundTweaks;

public class HarmonyPatchController : IInitializable, IDisposable
{
    private Harmony harmony;

    private HarmonyPatchController()
    {
        harmony = new Harmony("com.galaxymaster.hitsoundtweaks");
    }
    
    public void Initialize()
    {
        harmony.PatchAll(Plugin.ExecutingAssembly);
    }

    public void Dispose()
    {
        harmony.UnpatchSelf();
    }
}