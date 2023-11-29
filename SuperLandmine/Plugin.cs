using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace SuperLandmine
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource log;
        public Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        private void Awake()
        {
            log = base.Logger;
            log.LogInfo($"Loading plugin {PluginInfo.PLUGIN_NAME} ...");
            harmony.PatchAll();
            log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} loaded!");
        }
    }
    class PluginInfo
    {
        public const string PLUGIN_GUID = "com.github.phawitpp.superlandmine";
        public const string PLUGIN_NAME = "SuperLandmine";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}