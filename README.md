# RustStarterKitPlugin
A starter kit plugin for Rust
<br>
## Example Configuration
{<br>
  "Enabled": true, // Global switch for starter kit <br>
  "CommandsToRunOnSpawnEnabled": true, // Switch for commands to be run on INITIAL spawn <br>
  "CommandsToRunOnSpawn": ["recycler.give {0}"], // The commands to be run on INITIAL spawn. {0} -> UserId is injected where this is placed <br>
  "CommandsToRunOnRespawnEnabled": false, // The commands to be run on respawn <br>
  "CommandsToRunOnRespawn": ["recycler.give {0}"], // The commands to be run on respawn. {0} -> UserId is injected where this is placed <br>
  "ItemsToAddToInventoryOnSpawnEnabled": false, // Switch to add items to inventory on INITIAL spawn <br>
  "ItemsToAddToInventoryOnSpawn": [], // The items to add to inventory on INITIAL spawn eg. {"shortcode":"rifle.ak", "amount":1} <br>
  "ItemsToAddToInventoryOnRespawnEnabled": false, // Switch to add items to inventory on respawn <br>
  "ItemsToAddToInventoryOnRespawn": [] // The items to add to inventory on respawn eg. {"shortcode":"rifle.ak", "amount":1} <br>
} <br>
