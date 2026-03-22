using MelonLoader;
using HarmonyLib;

[assembly: MelonInfo(typeof(IKnowAGuyMaxPlayersMod.MaxPlayersMod), "Max Players Mod", "1.0.0", "Pandaismyname1")]
[assembly: MelonGame("BunchoNerds", "I Know a Guy")]

namespace IKnowAGuyMaxPlayersMod;

public class MaxPlayersMod : MelonMod
{
    public static MelonPreferences_Category Category { get; private set; }
    public static MelonPreferences_Entry<int> MaxPlayers { get; private set; }
    public static MelonPreferences_Entry<bool> DebugMode { get; private set; }

    public override void OnInitializeMelon()
    {
        Category = MelonPreferences.CreateCategory("MaxPlayersMod");
        MaxPlayers = Category.CreateEntry("MaxPlayers", 8, "Max Players",
            "Maximum number of players in lobby (game default is 4)");
        DebugMode = Category.CreateEntry("DebugMode", false, "Debug Mode",
            "Enable F5 network diagnostics overlay in-game");

        MelonLogger.Msg($"Max Players Mod loaded! Max players set to {MaxPlayers.Value}");

        Patches.UIPatches.Apply(HarmonyInstance);
    }

    public override void OnUpdate()
    {
        DebugSpawner.OnUpdate();
    }
}
