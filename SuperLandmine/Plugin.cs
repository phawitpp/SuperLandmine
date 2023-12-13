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
        public static ConfigEntry<int> config_LandmineMinAmount;
        public static ConfigEntry<int> config_LandmineMaxAmount;
        public static ConfigEntry<bool> config_EnableLandmineSound;
        public static ConfigEntry<bool> config_EnemyCanTriggerLandmine;
        public static ConfigEntry<bool> config_LandmineCanSpawnOutside;
        public static ConfigEntry<bool> config_UseDefaultLandmineSpawnRate;
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
            config_LandmineMinAmount = ((BaseUnityPlugin)this).Config.Bind<int>("Landmine min amount", "Value", 10, "Min landmine to be spawned in the map");
            config_LandmineMaxAmount = ((BaseUnityPlugin)this).Config.Bind<int>("Landmine max amount", "Value", 15, "Max landmine to be spawned in the map");
            config_EnableLandmineSound = ((BaseUnityPlugin)this).Config.Bind<bool>("Disable landmine sound", "Value", true, "Enable or disable landmine sound");
            config_EnemyCanTriggerLandmine = ((BaseUnityPlugin)this).Config.Bind<bool>("Enemy trigger landmine", "Value", true, "Enable or disable enemy can trigger landmine");
            config_LandmineCanSpawnOutside = ((BaseUnityPlugin)this).Config.Bind<bool>("Landmine can spawn outside", "Value", true, "Enable or disable landmine can spawn outside");
            config_UseDefaultLandmineSpawnRate = ((BaseUnityPlugin)this).Config.Bind<bool>("Use default landmine spawn rate", "Value", false, "Enable or disable default landmine spawn rate");

        }
    }
    class PluginInfo
    {
        public const string PLUGIN_GUID = "Superlandmine";
        public const string PLUGIN_NAME = "SuperLandmine";
        public const string PLUGIN_VERSION = "1.1.2";
    }

}