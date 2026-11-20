The game supports both transports Unity and Steam. To switch transport set UseSteam checkbox in Assets\Game\Config\GameNetworkConfig.asset file. To create Steam build launch build with checked checkbox. File "steam_appid.txt" isn't needed.

In Steam build "Join" button just opens Steam Friends list. To start Steam multiplayer game use Steam UI is necessary. Let's assume that host (H) and client (C) have already started Steam Desctop as well as Steam build of the game. Next steps are:
1. H click "Create"
2. H invites C via Steam Friends UI to join the game
3. C accepts invite and focus game window back
4. After several seconds C will join the game, no futher actions are needed

Steam transport code source: https://github.com/Unity-Technologies/multiplayer-community-contributions/tree/main/Transports/com.community.netcode.transport.facepunch

Architecture:
General ideas are loose coupling between services and clear areas of responsibility of classes. Core gameplay logic doesn't depend on how player input was caught, UI logic doesn't not tied to low level network implemetation and so on. Also the code is divided into logical blocks, on the basis of which it can be split into separate assemblies.
Steam-specific code interacts with other classes via interfaces, so it's possible implement differently and add another network transport. To add new interactable item it's needed to create new prefab variant, configure it's mesh and collider, and add it to network prefabs config. There is also PlayerSpawner config, so new item can be tested in game without code changes.

Network:
Player movement and camera use owner-authorative approach for input responsiveness. Server grants ownership to grabbed items to player for quick responses to player input also. Throwed items are no longer depends on player input and may interact with server-controlled or other players-controlled objects, so they are processed on server. All player network communication groupped in PlayerNetwork class for ease of maintain.

Knowing issues:
- When player leaves Steam session for other players he still remains in the game for a noticeable time. May be Steam doesn't notified that the player has left by himself and is waiting for connection timeout.
- Connection service has a dependency on the gameplay loop: "non-network scene - lobby - game scene". In my opinion this isn't connection manager's responsibility and should be separated into other classes.

Controls:
- Movement: WASD
- Camera: Mouse move
- Grab/Release: Mouse Left
- Throw: Mouse Right
- Spawn box: Q
- Delete grabbed box: C
- Player list: Hold TAB
