using System.Collections;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace SuperLandmine.Patchs

{
    [HarmonyPatch(typeof(Landmine))]
    public class SuperLandminePatch
    {

        [HarmonyPatch("PressMineClientRpc")]
        [HarmonyPostfix]
        public static void playAtomicSound(Landmine __instance)
        {

            Plugin.log.LogInfo(">>>>>>>>>>Play atomic sound");
            __instance.mineAudio.volume = 1.5f;
            __instance.mineFarAudio.volume = 1.5f;
            string path = "file://" + Paths.PluginPath + "\\SuperLandmine\\atomic.mp3";
            UnityWebRequest audioClip = UnityWebRequestMultimedia.GetAudioClip(path, (AudioType)13);
            audioClip.SendWebRequest();
            while (!audioClip.isDone)
            {
            }
            AudioClip clip = DownloadHandlerAudioClip.GetContent(audioClip);
            __instance.mineAudio.PlayOneShot(clip, 1.5f);
        }

        [HarmonyPatch("Detonate")]
        [HarmonyPrefix]
        public static void IncreaseAudioVolume(Landmine __instance)
        {
            Plugin.log.LogInfo("Increase landmine volume and kill range");
            __instance.StartCoroutine(DelayedActions(__instance));
            return;
        }

        private static IEnumerator DelayedActions(Landmine __instance)
        {
            yield return new WaitForSeconds(2);
            __instance.mineAudio.volume = 1.5f;
            __instance.mineFarAudio.volume = 1.5f;
            __instance.mineAudio.pitch = Random.Range(0.93f, 1.2f);
            __instance.mineAudio.PlayOneShot(__instance.mineDetonate, 1.5f);
            Landmine.SpawnExplosion(__instance.transform.position + Vector3.up, spawnExplosionEffect: true, 10.0f, 12.0f);
            yield return new WaitForSeconds(2);
        }

        [HarmonyPatch("StartIdleAnimation")]
        [HarmonyPostfix]
        public static void increaseIdleAnimationSpeed(Landmine __instance)
        {
            Plugin.log.LogInfo("Disable minevolume");
            __instance.mineAudio.volume = 0f;
            __instance.mineFarAudio.volume = 0f;
        }

    }

    [HarmonyPatch(typeof(RoundManager))]
    public class RoundManagerPatch
    {
        [HarmonyPatch("LoadNewLevel")]
        [HarmonyPrefix]
        public static void spawnLanmine(ref SelectableLevel newLevel)
        {
            Plugin.log.LogInfo(">>>>>>>>>>>Load landmine");
            SelectableLevel selectableLevel = newLevel;
            SpawnableMapObject[] spawnableMapObjects = selectableLevel.spawnableMapObjects;
            foreach (SpawnableMapObject spawnObject in spawnableMapObjects)
            {
                if ((Object)(object)spawnObject.prefabToSpawn.GetComponentInChildren<Landmine>() != (Object)null)
                {
                    Plugin.log.LogInfo(">>>>>>>>>>Initial landmine");
                    spawnObject.numberToSpawn = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
                    {
                                new Keyframe(0f, 15f),
                                new Keyframe(1f, 17f)
                    });
                }
            }
        }
    }
}


