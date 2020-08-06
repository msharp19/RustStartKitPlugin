# Rust Starter Kit Plugin
A starter kit plugin for Rust

## Features
- This enables users to run commands upon respawn.
- The commands can get the userId injected into them by adding "{0}"
- This enables users to get any inventory item/s upon respawn.
- The inventory items can be given specifying short code/amount
- Cool down periods are supplied to both commands and inventory items and they are configurable
- The Startup kit cooldown data file is refreshed on new .sav reseting all cooldown periods (per wipe)
- Shows a configurable message on respawn
- Allows the removal of existing inventory (torch/stone)
## Example Configuration
```json
{
 /* Switch for the plugin */
  "Enabled": true,
  /* Switch for command execution on respawn */
  "CommandsToRunOnRespawnEnabled": true,
  /* The list of commands to run for player on respawn */
  "CommandsToRunOnRespawn":[ 
      {
           /* The command text to execute (here is injecting userId with {0} */
          "Text": "recycler.give {0}",
          /*  If you only want to run this once per wipe then you can set to this */
          "CoolDownPeriodInSeconds": 999999999
      }
  ],
  /* Switch to give items specified below */
  "ItemsToAddToInventoryOnRespawnEnabled": true,
  /* List of items to give players */
  "ItemsToAddToInventoryOnRespawn": [
      {
          /* The short code of the item */
          "ShortCode": "pumpkin",
          /* Amount to give */
          "Amount": 2,
          /* The cooldown period is 0 here which means always given each respawn */
          "CoolDownPeriodInSeconds": 0
      },
      {
          "ShortCode": "bow.hunting",
          "Amount": 1,
          /* If respawn happens twice in 60 seconds then a bow will not be included on respawn */
          "CoolDownPeriodInSeconds": 60
      },
      {
          "ShortCode": "arrow.wooden",
          "Amount": 5,
          "CoolDownPeriodInSeconds": 60
      }
  ],
  /* Switch to show player respawn message */
  "ShowRespawnMessage": false,
  /* The message to set */
  "RespawnMessage": "Heres your starter kit!",
  /* Switch to remove any existing inventory */
  "RemoveExistingInventory": false
}
```