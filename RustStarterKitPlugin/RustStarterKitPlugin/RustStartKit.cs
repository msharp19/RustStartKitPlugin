using Oxide.Core.Libraries.Covalence;
using System;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    /// <summary>
    /// Plugin that allows Rust starter kits
    /// </summary>
    [Info("RustStarterKit", "matta", "1.0")]
    public class RustStartKit : RustPlugin
    {
        private StartKitConfig _config;

        #region Hooks 

        /// <summary>
        /// Called on init of plugin
        /// </summary>
        void Init()
        {
            Puts("RustStarterKit loaded.");
        }

        /// <summary>
        /// Called on player first spawn
        /// </summary>
        /// <param name="player">The player spawned</param>
        /// <returns></returns>
        object OnPlayerSpawn(BasePlayer player)
        {
            // Check the start kit is enabled
            if (_config.Enabled)
            {
                // Check if commands are to be run
                if (_config.CommandsToRunOnSpawnEnabled)
                    RunCommands(player, _config.CommandsToRunOnSpawn);

                // Add inventory items defined in config
                if(_config.ItemsToAddToInventoryOnSpawnEnabled)
                    AddInventoryItems(player, _config.ItemsToAddToInventoryOnSpawn);
            }

            return null;
        }

        /// <summary>
        /// Called on player respawn
        /// </summary>
        /// <param name="player"></param>
        void OnUserRespawned(IPlayer player)
        {
            if (_config.Enabled)
            {
                // Convert to base player
                var respawnPlayer = player.Object as BasePlayer;

                // Check if commands are to be run
                if (_config.CommandsToRunOnRespawnEnabled)
                    RunCommands(respawnPlayer, _config.CommandsToRunOnRespawn);

                // Add inventory items defined in config
                if (_config.ItemsToAddToInventoryOnRespawnEnabled)
                    AddInventoryItems(respawnPlayer, _config.ItemsToAddToInventoryOnRespawn);
            }
        }

        #endregion Hooks

        #region Helpers

        /// <summary>
        /// Runs commands (defined in config)
        /// </summary>
        /// <param name="player">The player to run predetermined commands for</param>
        /// <param name="commands">The commands to run</param>
        private void RunCommands(BasePlayer player, List<string> commands)
        {
            Puts("Running commands");

            // Run the commands one by one
            foreach (var command in commands)
            {
                // Run command - add player id if required {0}
                Server.Command(string.Format(command, player.UserIDString));
                // Log command run
                Puts($"Command: {command} executed at: {DateTime.UtcNow}UTC");
            }
        }

        /// <summary>
        /// Adds inventory items to a player
        /// </summary>
        /// <param name="player">The player to add inventory items to</param>
        /// <param name="inventoryItems">The inventory items to add</param>
        private void AddInventoryItems(BasePlayer player, List<InventoryItem> inventoryItems)
        {
            // Log action
            Puts($"Setting inventory for player:{player.UserIDString}");

            // Add items to inventory
            foreach (var inventoryItem in inventoryItems)
            {
                try
                {
                    // Safely try to add inventory item
                    player.inventory.GiveItem(ItemManager.CreateByName(inventoryItem.ShortCode, inventoryItem.Amount));
                }
                catch
                {
                    // If error - log it but then we can continue to add the rest of the inventory items
                    Puts($"Error adding inventory item:{inventoryItem.ShortCode} with amount:{inventoryItem.Amount}");
                }
            }
        }

        /// <summary>
        /// Loads start kit config
        /// </summary>
        protected override void LoadConfig()
        {
            // Load base config
            base.LoadConfig();

            // Try to load starter kit config (may have errors)
            try
            {
                // Try to read config
                _config = Config.ReadObject<StartKitConfig>();

                // If empty then load the default
                if (_config == null) 
                    LoadDefaultConfig();
            }
            catch
            {
                // Log if read error
                PrintError($"An error occurred reading the configuration file!");
                PrintError($"Check it with any JSON Validator!");
                return;
            }

            // Save the config
            Config.WriteObject(_config);
        }

        /// <summary>
        /// The default config is loaded in constructor (everything turned off)
        /// </summary>
        protected override void LoadDefaultConfig() 
            => _config = new StartKitConfig();

        #endregion Helpers
    }

    #region Helper Classes

    /// <summary>
    /// Config class
    /// </summary>
    public class StartKitConfig
    {
        /// <summary>
        /// If the start kit is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// List of commands to run as part of starter kit (ie. can give recycler) on spawn
        /// </summary>
        public List<string> CommandsToRunOnSpawn { get; set; }

        /// <summary>
        /// List of commands to run as part of starter kit (ie. can give recycler) on respawn
        /// </summary>
        public List<string> CommandsToRunOnRespawn { get; set; }

        /// <summary>
        /// Allows for the commands to be turned of (if not needed) for on spawn command actions
        /// </summary>
        public bool CommandsToRunOnSpawnEnabled { get; set; }

        /// <summary>
        /// Items to add to the inventory on spawn
        /// </summary>
        public List<InventoryItem> ItemsToAddToInventoryOnSpawn { get; set; }

        /// <summary>
        /// Allows items to be added to player inventory on spawn to be turned of (if not needed)
        /// </summary>
        public bool ItemsToAddToInventoryOnSpawnEnabled { get; set; }

        /// <summary>
        /// Allows for the commands to be turned of (if not needed) for on respawn command actions
        /// </summary>
        public bool CommandsToRunOnRespawnEnabled { get; set; }

        /// <summary>
        /// Allows items to be added to player inventory on respawn to be turned of (if not needed)
        /// </summary>
        public bool ItemsToAddToInventoryOnRespawnEnabled { get; set; }

        /// <summary>
        /// Items to add to the inventory on respawn
        /// </summary>
        public List<InventoryItem> ItemsToAddToInventoryOnRespawn { get; set; }

        /// <summary>
        /// Constructor - sets defaults
        /// </summary>
        public StartKitConfig()
        {
            // Defaults (off and empty)
            Enabled = false;
            ItemsToAddToInventoryOnSpawnEnabled = false;
            ItemsToAddToInventoryOnRespawnEnabled = false;
            ItemsToAddToInventoryOnSpawn = new List<InventoryItem>();
            ItemsToAddToInventoryOnRespawn = new List<InventoryItem>();
            CommandsToRunOnSpawnEnabled = false;
            CommandsToRunOnRespawnEnabled = false;
            CommandsToRunOnRespawn = new List<string>();
            CommandsToRunOnSpawn = new List<string>();
        }
    }

    /// <summary>
    /// An inventory item to give
    /// </summary>
    public class InventoryItem
    {
        /// <summary>
        /// The item shortcode eg. ammo.shotgun.fire
        /// </summary>
        public string ShortCode { get; set; }

        /// <summary>
        /// The number of shortcode item to give
        /// </summary>
        public int Amount { get; set; }
    }

    #endregion Helper Classes
}