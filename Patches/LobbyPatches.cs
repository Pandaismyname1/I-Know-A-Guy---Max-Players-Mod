using HarmonyLib;
using MelonLoader;
using Il2CppNetwork;

namespace IKnowAGuyMaxPlayersMod.Patches;

/// <summary>
/// Patches for lobby creation and max player reporting.
///
/// Flow: HostPageController.HostLobbyAsync(name, privacy, maxPlayers)
///   -> NetworkController.HostLobby(name, maxPlayers, isPrivate)
///   -> LobbyManager.CreateLobby(CreateLobbyOptions { MaxPlayers = ... })
///   -> SteamMatchmaking.CreateLobby(lobbyType, maxPlayers)
/// </summary>
public static class LobbyPatches
{
    [HarmonyPatch(typeof(Il2CppUI.MainMenu.HostPageController), nameof(Il2CppUI.MainMenu.HostPageController.HostLobbyAsync))]
    [HarmonyPrefix]
    public static void HostLobbyAsync_Prefix(ref int maxPlayers)
    {
        var configured = MaxPlayersMod.MaxPlayers.Value;
        MelonLogger.Msg($"[LobbyPatches] HostLobbyAsync: overriding maxPlayers {maxPlayers} -> {configured}");
        maxPlayers = configured;
    }

    [HarmonyPatch(typeof(NetworkController), nameof(NetworkController.HostLobby))]
    [HarmonyPrefix]
    public static void HostLobby_Prefix(ref int maxPlayers)
    {
        var configured = MaxPlayersMod.MaxPlayers.Value;
        MelonLogger.Msg($"[LobbyPatches] NetworkController.HostLobby: overriding maxPlayers {maxPlayers} -> {configured}");
        maxPlayers = configured;
    }

    [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.CreateLobby))]
    [HarmonyPrefix]
    public static void CreateLobby_Prefix(CreateLobbyOptions options)
    {
        if (options != null)
        {
            var configured = MaxPlayersMod.MaxPlayers.Value;
            MelonLogger.Msg($"[LobbyPatches] CreateLobby: overriding options.MaxPlayers {options.MaxPlayers} -> {configured}");
            options.MaxPlayers = configured;
        }
    }

    [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.CurrentLobbyMaxPlayers), MethodType.Getter)]
    [HarmonyPostfix]
    public static void CurrentLobbyMaxPlayers_Postfix(ref int __result)
    {
        if (__result > 0)
        {
            __result = MaxPlayersMod.MaxPlayers.Value;
        }
    }

    [HarmonyPatch(typeof(NetworkController), nameof(NetworkController.CurrentLobbyMaxPlayers), MethodType.Getter)]
    [HarmonyPostfix]
    public static void NetworkController_CurrentLobbyMaxPlayers_Postfix(ref int __result)
    {
        if (__result > 0)
        {
            __result = MaxPlayersMod.MaxPlayers.Value;
        }
    }
}
