using HarmonyLib;
using UnityEngine;
namespace SuperLandmine.Patchs;

[HarmonyPatch(typeof(Landmine))]
public class SuperLandminePatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void DisableLandmineSound(ref AudioClip ___mineAudio, ref AudioClip ___mineFarAudio)
    {
        ___mineAudio.UnloadAudioData();
        ___mineFarAudio.UnloadAudioData();
        Plugin.mls.LogInfo("Disabled landmine sound successfully!");

    }

    [HarmonyPatch("Detonate")]
    [HarmonyPrefix]
    public static void increaseLandmineRadius(Landmine __instance)
    {
        Landmine.SpawnExplosion(__instance.transform.position, spawnExplosionEffect: true, killRange: 10.0f, damageRange: 12.0f);
        Plugin.mls.LogInfo("Detonated landmine");
    }

}
