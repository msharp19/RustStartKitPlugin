# RustStarterKitPlugin
A starter kit plugin for Rust

## Example Configuration
{
  "Enabled": true, // Global switch for starter kit
  "CommandsToRunOnSpawnEnabled": true, // Switch for commands to be run on INITIAL spawn
  "CommandsToRunOnSpawn": ["recycler.give {0}"], // The commands to be run on INITIAL spawn. {0} -> UserId is injected where this is placed
  "CommandsToRunOnRespawnEnabled": false, // The commands to be run on respawn
  "CommandsToRunOnRespawn": ["recycler.give {0}"], // The commands to be run on respawn. {0} -> UserId is injected where this is placed
  "ItemsToAddToInventoryOnSpawnEnabled": false, // Switch to add items to inventory on INITIAL spawn
  "ItemsToAddToInventoryOnSpawn": [], // The items to add to inventory on INITIAL spawn eg. {"shortcode":"rifle.ak", "amount":1}
  "ItemsToAddToInventoryOnRespawnEnabled": false, // Switch to add items to inventory on respawn
  "ItemsToAddToInventoryOnRespawn": [] // The items to add to inventory on respawn eg. {"shortcode":"rifle.ak", "amount":1}
}
