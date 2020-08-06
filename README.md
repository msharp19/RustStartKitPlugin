# RustStarterKitPlugin
A starter kit plugin for Rust

## Features
- This enables users to run commands upon respawn.
- The commands can get the userId injected into them by adding "{0}"
- This enables users to get any inventory item/s upon respawn.
- The inventory items can be given specifying short code/amount
- Cool down periods are supplied to both commands and inventory items and they are configurable
- The Startup kit cooldown data file is refreshed on new .sav (resets all cooldown periods)
- Shows a configurable message on respawn
- Allows the removal of existing inventory (torch/stone)
## Example Configuration
```json
{
  "Enabled": true, // Switch for the plugin
  "CommandsToRunOnRespawnEnabled": true, // Switch for command execution on respawn
  "CommandsToRunOnRespawn":[ // The list of commands to run for player on respawn
      {
          "Text": "recycler.give {0}", // The command text to execute (here is injecting userId with {0}
          "CoolDownPeriodInSeconds": 999999999 // If you only want to run this once per wipe then you can set to this
      }
  ],
  "ItemsToAddToInventoryOnRespawnEnabled": true, // Switch to give items specified below
  "ItemsToAddToInventoryOnRespawn": [ // List of items to give players
      {
          "ShortCode": "pumpkin", // The short code of the item
          "Amount": 2, // Amount to give
          "CoolDownPeriodInSeconds": 0 // The cooldown period is 0 here which means always given each respawn
      },
      {
          "ShortCode": "bow.hunting",
          "Amount": 1,
          "CoolDownPeriodInSeconds": 60 // If respawn happens twice in 60 seconds then a bow will not be included on respawn
      },
      {
          "ShortCode": "arrow.wooden",
          "Amount": 5,
          "CoolDownPeriodInSeconds": 60
      }
  ],
  "ShowRespawnMessage": false, // Switch to show player respawn message
  "RespawnMessage": "Heres your starter kit!", // The message to set
  "RemoveExistingInventory": false // Switch to remove any existing inventory
}
```