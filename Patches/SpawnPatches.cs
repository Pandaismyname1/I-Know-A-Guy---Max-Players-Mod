using HarmonyLib;
using MelonLoader;
using Il2CppNetwork;
using UnityEngine;

namespace IKnowAGuyMaxPlayersMod.Patches;

/// <summary>
/// Patches for LobbyPlayerSpawner to handle more players than there are spawn points.
/// If the index exceeds the spawn point count, we wrap around with a small offset
/// so players don't stack on top of each other.
/// </summary>
public static class SpawnPatches
{
    [HarmonyPatch(typeof(LobbyPlayerSpawner), nameof(LobbyPlayerSpawner.GetSpawnPosition))]
    [HarmonyPrefix]
    public static bool GetSpawnPosition_Prefix(LobbyPlayerSpawner __instance, int index, ref Vector3 __result)
    {
        var spawnPoints = __instance.spawnPoints;
        if (spawnPoints == null || spawnPoints.Count == 0)
            return true;

        if (index < spawnPoints.Count)
            return true;

        int wrappedIndex = index % spawnPoints.Count;
        int wrapCount = index / spawnPoints.Count;

        var baseTransform = spawnPoints._items[wrappedIndex];
        if (baseTransform == null)
            return true;

        var offset = baseTransform.right * (1.5f * wrapCount);
        __result = baseTransform.position + offset;

        MelonLogger.Msg($"[SpawnPatches] Extra spawn point for index {index}: wrapped to {wrappedIndex} + offset (cycle {wrapCount})");
        return false;
    }

    [HarmonyPatch(typeof(LobbyPlayerSpawner), nameof(LobbyPlayerSpawner.GetSpawnRotation))]
    [HarmonyPrefix]
    public static bool GetSpawnRotation_Prefix(LobbyPlayerSpawner __instance, int index, ref Quaternion __result)
    {
        var spawnPoints = __instance.spawnPoints;
        if (spawnPoints == null || spawnPoints.Count == 0)
            return true;

        if (index < spawnPoints.Count)
            return true;

        int wrappedIndex = index % spawnPoints.Count;
        var baseTransform = spawnPoints._items[wrappedIndex];
        if (baseTransform == null)
            return true;

        __result = baseTransform.rotation;
        return false;
    }
}
