using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace SuperLandmine
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        public static ManualLogSource mls;
        private void Awake()
        {
            Logger.LogInfo($"Loading plugin {PluginInfo.PLUGIN_GUID} ...");
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} loaded!");
        }
    }
    class PluginInfo
    {
        public const string PLUGIN_GUID = "com.github.phawitpp.superlandmine";
        public const string PLUGIN_NAME = "SuperLandmine";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}