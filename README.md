# RustStarterKitPlugin
A starter kit plugin for Rust
<br>
## Example Configuration
{<br>
  &nbsp;&nbsp;"Enabled": true,<br>
  &nbsp;&nbsp;"CommandsToRunOnRespawnEnabled": true,<br>
  &nbsp;&nbsp;"CommandsToRunOnRespawn":[<br>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Text": "recycler.give {0}",<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"CoolDownPeriodInSeconds": 999999999<br>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<br>
  &nbsp;&nbsp;],<br>
  &nbsp;&nbsp;"ItemsToAddToInventoryOnRespawnEnabled": true,<br>
  &nbsp;&nbsp;"ItemsToAddToInventoryOnRespawn": [<br>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"ShortCode": "pumpkin",<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Amount": 2,<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"CoolDownPeriodInSeconds": 0<br>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},<br>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"ShortCode": "bow.hunting",<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Amount": 1,<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"CoolDownPeriodInSeconds": 60<br>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;},<br>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"ShortCode": "arrow.wooden",<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"Amount": 5,<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"CoolDownPeriodInSeconds": 60<br>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}<br>
  &nbsp;&nbsp;],<br>
  &nbsp;&nbsp;"ShowRespawnMessage": false,<br>
  &nbsp;&nbsp;"RespawnMessage": "",<br>
  &nbsp;&nbsp;"RemoveExistingInventory": false<br>
}<br>
