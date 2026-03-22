using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using Il2CppSystem.Collections.Generic;

namespace IKnowAGuyMaxPlayersMod.Patches;

/// <summary>
/// Patches HostPageController to extend the max players dropdown
/// beyond the default 4-player options.
/// Uses manual Harmony patching for better error visibility with IL2CPP.
/// </summary>
public static class UIPatches
{
    private static readonly Type HostPageType = typeof(Il2CppUI.MainMenu.HostPageController);

    public static void Apply(HarmonyLib.Harmony harmony)
    {
        try
        {
            // Try patching Show
            var showMethod = AccessTools.Method(HostPageType, "Show");
            if (showMethod != null)
            {
                harmony.Patch(showMethod,
                    postfix: new HarmonyMethod(typeof(UIPatches), nameof(OnShowPostfix)));
                MelonLogger.Msg("[UIPatches] Patched Show");
            }
            else
            {
                MelonLogger.Warning("[UIPatches] Could not find Show method");
            }

            // Try patching Initialize
            var initMethod = AccessTools.Method(HostPageType, "Initialize");
            if (initMethod != null)
            {
                harmony.Patch(initMethod,
                    postfix: new HarmonyMethod(typeof(UIPatches), nameof(OnInitializePostfix)));
                MelonLogger.Msg($"[UIPatches] Patched Initialize (params: {string.Join(", ", initMethod.GetParameters().Select(p => p.ParameterType.Name))})");
            }
            else
            {
                MelonLogger.Warning("[UIPatches] Could not find Initialize method");
            }

            // Try patching QueryElements
            var queryMethod = AccessTools.Method(HostPageType, "QueryElements");
            if (queryMethod != null)
            {
                harmony.Patch(queryMethod,
                    postfix: new HarmonyMethod(typeof(UIPatches), nameof(OnQueryElementsPostfix)));
                MelonLogger.Msg("[UIPatches] Patched QueryElements");
            }
            else
            {
                MelonLogger.Warning("[UIPatches] Could not find QueryElements method");
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[UIPatches] Failed to apply patches: {ex}");
        }
    }

    public static void OnShowPostfix(Il2CppUI.MainMenu.HostPageController __instance)
    {
        PatchDropdown(__instance, "Show");
    }

    public static void OnInitializePostfix(Il2CppUI.MainMenu.HostPageController __instance)
    {
        PatchDropdown(__instance, "Initialize");
    }

    public static void OnQueryElementsPostfix(Il2CppUI.MainMenu.HostPageController __instance)
    {
        PatchDropdown(__instance, "QueryElements");
    }

    private static void PatchDropdown(Il2CppUI.MainMenu.HostPageController instance, string caller)
    {
        try
        {
            var dropdown = instance.maxPlayersDropdown;
            if (dropdown == null)
            {
                MelonLogger.Warning($"[UIPatches] {caller}: maxPlayersDropdown is null");
                return;
            }

            var maxPlayers = MaxPlayersMod.MaxPlayers.Value;

            var choices = new List<string>();
            for (int i = 2; i <= maxPlayers; i++)
            {
                choices.Add(i.ToString());
            }

            dropdown.choices = choices;
            dropdown.value = maxPlayers.ToString();

            MelonLogger.Msg($"[UIPatches] {caller}: Dropdown updated with choices 2-{maxPlayers}");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[UIPatches] {caller}: Failed: {ex}");
        }
    }
}
