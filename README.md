# Max Players Mod for I Know a Guy

A MelonLoader mod that increases the maximum lobby player count beyond the default limit of 4.

## Features

- **Configurable max players** — set any limit from 2 to your desired max (default: 8)
- **Extended lobby UI** — the "Max Players" dropdown in Create Lobby automatically shows all options up to your configured limit
- **Spawn point wrapping** — when player count exceeds available spawn points, extra players spawn at offset positions so nobody stacks on top of each other
- **Debug diagnostics** — optional F5 keybind to dump network/lobby state to the MelonLoader log (disabled by default)

## Requirements

- [MelonLoader](https://melonwiki.xyz/) v0.7.x (Open-Beta)
- I Know a Guy (Demo or Full) — tested on V0.76, Unity 6000.3.10f1

## Installation

1. Install MelonLoader for your game if you haven't already
2. Download `IKnowAGuyMaxPlayersMod.dll` from [Releases](https://github.com/Pandaismyname1/IKnowAGuyMaxPlayersMod/releases)
3. Place the DLL in your game's `Mods/` folder:
   ```
   I Know a Guy/
   └── Mods/
       └── IKnowAGuyMaxPlayersMod.dll
   ```
4. Launch the game — the mod loads automatically

## Configuration

After first launch, the mod creates a config section in `UserData/MelonPreferences.cfg`:

```ini
[MaxPlayersMod]
# Maximum number of players in lobby (game default is 4)
MaxPlayers = 8
# Enable F5 network diagnostics overlay in-game
DebugMode = false
```

- **MaxPlayers** — the maximum player count for lobbies you host (2–any). The Create Lobby dropdown will show options from 2 up to this value.
- **DebugMode** — when `true`, press F5 in-game to log current network state (server status, connected clients, lobby max players).

## How It Works

The mod uses Harmony to patch three layers of the lobby creation flow:

1. **UI layer** (`HostPageController`) — extends the max players dropdown choices
2. **Network layer** (`NetworkController.HostLobby`) — overrides the max players parameter
3. **Lobby layer** (`LobbyManager.CreateLobby`) — sets `CreateLobbyOptions.MaxPlayers` to the configured value
4. **Runtime getters** — patches `CurrentLobbyMaxPlayers` on both `LobbyManager` and `NetworkController` to return the modded value
5. **Spawn handling** — wraps `LobbyPlayerSpawner.GetSpawnPosition/GetSpawnRotation` to cycle through spawn points with offsets for overflow players

## Building from Source

Requires .NET 6 SDK.

```bash
# Clone the repo into your game directory (next to MelonLoader/)
git clone https://github.com/Pandaismyname1/IKnowAGuyMaxPlayersMod.git

# Build
cd IKnowAGuyMaxPlayersMod
dotnet build -c Release

# Deploy
cp bin/Release/net6.0/IKnowAGuyMaxPlayersMod.dll ../Mods/
```

The project references MelonLoader and game assemblies via relative paths (`../MelonLoader/`), so it must be built from within the game directory.

## Known Limitations

- All players in the lobby should have the mod installed for the best experience
- Only the host's configured max players value is used for the lobby
- Spawn point wrapping uses simple positional offsets — players beyond the original spawn count may spawn in slightly awkward positions
- Tested as host only; client-side behavior with vanilla hosts is untested

## License

MIT License — see [LICENSE](LICENSE) for details.
