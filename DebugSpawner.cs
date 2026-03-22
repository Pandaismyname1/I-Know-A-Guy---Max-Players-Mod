using System;
using MelonLoader;
using UnityEngine.InputSystem;
using Il2CppNetwork;
using Unity.Netcode;

namespace IKnowAGuyMaxPlayersMod;

/// <summary>
/// Debug tool: F5 dumps lobby/network diagnostics to the log.
/// Only active when DebugMode is enabled in config.
/// </summary>
public static class DebugSpawner
{
    public static void OnUpdate()
    {
        if (!MaxPlayersMod.DebugMode.Value)
            return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.f5Key.wasPressedThisFrame)
            DumpNetworkInfo();
    }

    private static void DumpNetworkInfo()
    {
        try
        {
            MelonLogger.Msg("=== Network Diagnostics ===");

            var nm = NetworkManager.Singleton;
            if (nm == null)
            {
                MelonLogger.Msg("  NetworkManager: null (not in a session)");
                return;
            }

            MelonLogger.Msg($"  NetworkManager: IsServer={nm.IsServer}, IsHost={nm.IsHost}, IsClient={nm.IsClient}");
            MelonLogger.Msg($"  LocalClientId: {nm.LocalClientId}");
            MelonLogger.Msg($"  NetworkConfig.ConnectionApproval: {nm.NetworkConfig?.ConnectionApproval}");

            var lobbyMgr = UnityEngine.Object.FindObjectOfType<LobbyManager>();
            if (lobbyMgr != null)
                MelonLogger.Msg($"  LobbyManager.CurrentLobbyMaxPlayers: {lobbyMgr.CurrentLobbyMaxPlayers}");
            else
                MelonLogger.Msg("  LobbyManager: not found");

            var netCtrl = UnityEngine.Object.FindObjectOfType<NetworkController>();
            if (netCtrl != null)
                MelonLogger.Msg($"  NetworkController.CurrentLobbyMaxPlayers: {netCtrl.CurrentLobbyMaxPlayers}");
            else
                MelonLogger.Msg("  NetworkController: not found");

            MelonLogger.Msg($"  Mod MaxPlayers config: {MaxPlayersMod.MaxPlayers.Value}");
            MelonLogger.Msg("=== End Diagnostics ===");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[DebugSpawner] DumpNetworkInfo failed: {ex}");
        }
    }
}
