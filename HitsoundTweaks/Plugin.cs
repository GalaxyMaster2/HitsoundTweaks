using HitsoundTweaks.Configuration;
using HitsoundTweaks.Installers;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;

namespace HitsoundTweaks;

[Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable]
public class Plugin
{
    internal static IPALogger Log { get; private set; }
    internal static Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();

    [Init]
    /// <summary>
    /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
    /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
    /// Only use [Init] with one Constructor.
    /// </summary>
    public void Init(IPALogger logger, Config config, Zenjector zenjector)
    {
        Log = logger;
        var pluginConfig = config.Generated<PluginConfig>();

        zenjector.Install<AppInstaller>(Location.App, pluginConfig);
        zenjector.Install<MenuInstaller>(Location.Menu);
        zenjector.Install<PlayerInstaller>(Location.Player | Location.Tutorial);
    }
}