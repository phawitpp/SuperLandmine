using System.Collections;
using BepInEx;
using GameNetcodeStuff;
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
        public static void playTriggerSound(Landmine __instance)
        {
            Plugin.log.LogInfo("Play trigger sound");
            __instance.mineAudio.volume = 1.5f;
            __instance.mineFarAudio.volume = 1.5f;
            Traverse.Create(__instance).Field("pressMineDebounceTimer").SetValue(0.5f);
            __instance.mineAudio.PlayOneShot(__instance.minePress, 1.5f);
            WalkieTalkie.TransmitOneShotAudio(__instance.mineAudio, __instance.minePress);
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
            yield return new WaitForSeconds(0.5f);
            __instance.mineAudio.volume = 1.5f;
            __instance.mineFarAudio.volume = 1.5f;
            __instance.mineAudio.pitch = Random.Range(0.93f, 1.2f);
            __instance.mineAudio.PlayOneShot(__instance.mineDetonate, 1.35f);
            Landmine.SpawnExplosion(__instance.transform.position + Vector3.up, spawnExplosionEffect: true, 10.0f, 12.0f);
            yield return new WaitForSeconds(0.5f);

        }


        [HarmonyPatch("StartIdleAnimation")]
        [HarmonyPostfix]
        public static void increaseIdleAnimationSpeed(Landmine __instance)
        {
            Plugin.log.LogInfo("Disable minevolume");
            __instance.mineAudio.volume = 0f;
            __instance.mineFarAudio.volume = 0f;
        }


        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPrefix]
        public static void anyObjectTriggerLandmineEnter(ref Collider other, Landmine __instance)
        {
            float pressMineDebounceTimer = Traverse.Create(__instance).Field("pressMineDebounceTimer").GetValue<float>();

            if (__instance.hasExploded || pressMineDebounceTimer > 0f)
            {
                return;
            }
            Plugin.log.LogInfo("Object enter mine trigger, gameobject tag: " + other.gameObject.tag);
            if (other.CompareTag("Player"))
            {
                PlayerControllerB component = other.gameObject.GetComponent<PlayerControllerB>();
                if (!(component != GameNetworkManager.Instance.localPlayerController) && component != null && !component.isPlayerDead)
                {
                    Traverse.Create(__instance).Field("localPlayerOnMine").SetValue(true);
                    Traverse.Create(__instance).Field("pressMineDebounceTimer").SetValue(0.5f);
                    __instance.PressMineServerRpc();
                }
            }
            else
            {
                if (!other.CompareTag("PlayerRagdoll") && !other.CompareTag("PhysicsProp") && !other.CompareTag("Enemy"))
                {
                    return;
                }
                if ((bool)other.GetComponent<DeadBodyInfo>())
                {
                    if (other.GetComponent<DeadBodyInfo>().playerScript != GameNetworkManager.Instance.localPlayerController)
                    {
                        return;
                    }
                }
                else if ((bool)other.GetComponent<GrabbableObject>() && !other.GetComponent<GrabbableObject>().NetworkObject.IsOwner)
                {
                    return;
                }
                Traverse.Create(__instance).Field("pressMineDebounceTimer").SetValue(0.5f);

                __instance.PressMineServerRpc();
            }
            return;
        }

        [HarmonyPatch("OnTriggerExit")]
        [HarmonyPrefix]
        public static void anyObjectTriggerLandmineExit(ref Collider other, Landmine __instance)
        {
            bool mineActivated = Traverse.Create(__instance).Field("mineActivated").GetValue<bool>();

            if (__instance.hasExploded || !mineActivated)
            {
                return;
            }
            Plugin.log.LogInfo("Object leaving mine trigger, gameobject tag: " + other.gameObject.tag);
            if (other.CompareTag("Player"))
            {
                PlayerControllerB component = other.gameObject.GetComponent<PlayerControllerB>();
                if (component != null && !component.isPlayerDead && !(component != GameNetworkManager.Instance.localPlayerController))
                {
                    Traverse.Create(__instance).Field("localPlayerOnMine").SetValue(false);
                    Traverse.Create(__instance).Method("TriggerMineOnLocalClientByExiting").GetValue();
                }
            }
            else
            {
                if (!other.CompareTag("PlayerRagdoll") && !other.CompareTag("PhysicsProp") && !other.CompareTag("Enemy"))
                {
                    return;
                }
                if ((bool)other.GetComponent<DeadBodyInfo>())
                {
                    if (other.GetComponent<DeadBodyInfo>().playerScript != GameNetworkManager.Instance.localPlayerController)
                    {
                        return;
                    }
                }
                else if ((bool)other.GetComponent<GrabbableObject>() && !other.GetComponent<GrabbableObject>().NetworkObject.IsOwner)
                {
                    return;
                }
                Traverse.Create(__instance).Method("TriggerMineOnLocalClientByExiting").GetValue();
            }

            return;
        }

    }

    [HarmonyPatch(typeof(RoundManager))]
    public class RoundManagerPatch
    {
        [HarmonyPatch("LoadNewLevel")]
        [HarmonyPrefix]
        public static void spawnLanmine(ref SelectableLevel newLevel)
        {
            Plugin.log.LogInfo("Load landmine");
            SelectableLevel selectableLevel = newLevel;
            SpawnableMapObject[] spawnableMapObjects = selectableLevel.spawnableMapObjects;
            if (selectableLevel.spawnableMapObjects.Length != 0)
            {
                Plugin.log.LogInfo("Initial landmine inside");
                foreach (SpawnableMapObject spawnObject in spawnableMapObjects)
                {
                    if ((Object)(object)spawnObject.prefabToSpawn.GetComponentInChildren<Landmine>() != (Object)null)
                    {

                        spawnObject.numberToSpawn = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
                        {
                                new Keyframe(0f, Plugin.config_LandmineAmount.Value),
                                new Keyframe(1f, Plugin.config_LandmineAmount.Value)
                        });
                    }
                }
            }
            if (selectableLevel.spawnableOutsideObjects.Length != 0 && selectableLevel.spawnableOutsideObjects.Length != 0)
            {
                Plugin.log.LogInfo("Initial landmine outside");
                SpawnableOutsideObjectWithRarity[] spawnableOutsideObjectWithRarities = selectableLevel.spawnableOutsideObjects;
                foreach (SpawnableMapObject spawnObject in spawnableMapObjects)
                {
                    if ((Object)(object)spawnObject.prefabToSpawn.GetComponentInChildren<Landmine>() != (Object)null)
                    {

                        spawnableOutsideObjectWithRarities.AddItem(new SpawnableOutsideObjectWithRarity
                        {
                            spawnableObject = new SpawnableOutsideObject
                            {
                                prefabToSpawn = spawnObject.prefabToSpawn,
                            },
                            randomAmount = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
                    {
                            new Keyframe(0f, Plugin.config_LandmineAmount.Value),
                            new Keyframe(1f, Plugin.config_LandmineAmount.Value)
                    }),
                        });
                    }
                }


            }


        }
    }
}


