using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace SuperLandmine
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource log;
        public Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        public static ConfigEntry<int> config_LandmineAmount;
        private void Awake()
        {
            log = base.Logger;
            log.LogInfo($"Loading plugin {PluginInfo.PLUGIN_NAME} ...");
            ConfigSetup();
            harmony.PatchAll();
            log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} loaded!");
        }
        private void ConfigSetup()
        {
            config_LandmineAmount = ((BaseUnityPlugin)this).Config.Bind<int>("Landmine amount", "Value", 15, "How many landmine will be spawned in the map");
        }
    }
    class PluginInfo
    {
        public const string PLUGIN_GUID = "Superlandmine";
        public const string PLUGIN_NAME = "SuperLandmine";
        public const string PLUGIN_VERSION = "1.0.3";
    }

}