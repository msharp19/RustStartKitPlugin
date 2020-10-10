using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Plugins
{
    /// <summary>
    /// Plugin that allows Rust starter kits
    /// </summary>
    [Info("StarterKit", "matta", "1.0")]
    [Description("Starter kits for both new and existing players (spawn-respawn). This also includes the ability to run commands on spawn/respawn so this can be used in conjunction with other plugin commands ie. recycler.give USER")]
    public class StarterKit : RustPlugin
    {
        /// <summary>
        /// Class config
        /// </summary>
        private StarterKitConfig _config;

        /// <summary>
        /// Sotrage data
        /// </summary>
        private StoredData _storedData;

        /// <summary>
        /// Runtime user kits (applicable for when kits > 1)
        /// </summary>
        private IDictionary<string, string> _currentUserKits;

        #region Hooks 

        /// <summary>
        /// Called on init of plugin
        /// </summary>
        void Init()
        {
            // Check to unsubscribe to hooks if plugin is disabled
            if (!_config?.Enabled ?? false)
            {
                Unsubscribe(nameof(OnPlayerRespawn));
                return;
            }

            // Load persisted data
            _storedData = StoredDataService.LoadStoredData();

            // Setup current user kits (applicable for > 1 kit in selection list)
            _currentUserKits = new Dictionary<string, string>();
        }

        /// <summary>
        /// Called on new .sav file creation - reset stored data
        /// </summary>
        /// <param name="filename">File name to save</param>
        void OnNewSave(string filename)
        {
            // Overwrite current one
            _storedData = StoredDataService.ResetStoredData();
        }

        /// <summary>
        /// Called on player respawn
        /// </summary>
        /// <param name="player"></param>
        object OnPlayerRespawn(BasePlayer player)
        {
            // Check the start kit is enabled
            if (_config.Enabled)
            {
                // Select, build and add to player
                var kit = KitBuilder.SelectAndBuildKit(player.UserIDString, _config.RunType, _config.Kits);
                if (kit != null)
                    AddStarterKit(player, kit);
            }

            return player;
        }

        #endregion Hooks

        #region Helpers

        /// <summary>
        /// Creates a starter kit using config from commands and predetermined inventory items
        /// </summary>
        /// <param name="player">The player to equip</param>
        /// <param name="commands">Any commands to run (to map other plugins)</param>
        /// <param name="inventoryItems">And inventory items to provide players with</param>
        public void AddStarterKit(BasePlayer player, Kit kit)
        {
            // If we want to remove stone & torch
            if (kit.RemoveExistingInventory)
                player.inventory.Strip();

            // Check if commands are to be run
            if (kit.CommandsToRunOnRespawnEnabled)
                RunCommands(player, kit.CommandsToRunOnRespawn, out _);

            // Add inventory items defined in config
            if (kit.ItemsToAddToInventoryOnRespawnEnabled)
                AddInventoryItems(player, kit.ItemsToAddToInventoryOnRespawn, out _);

            // If we want to notify the user about the starter kit
            if (kit.ShowRespawnMessage)
                player.ChatMessage(kit.RespawnMessage);

            StoredDataService.SaveStoredData(_storedData);
        }

        /// <summary>
        /// Runs commands (defined in config)
        /// </summary>
        /// <param name="player">The player to run predetermined commands for</param>
        /// <param name="commands">The commands to run</param>
        /// <param name="storageHasBeenUpdated">If storage has been updated during runtime of this code</param>
        private void RunCommands(BasePlayer player, List<Command> commands, out bool storageHasBeenUpdated)
        {
            // Default update state
            storageHasBeenUpdated = false;

            // Run the commands one by one
            foreach (var command in commands)
            {
                try
                {
                    // Check that player isnt in the cool down period for current item
                    if (!IsPlayerInCoolDownPeriodForItem(player.UserIDString, command.Text))
                    {
                        // If the player is not in cooldown, run command and add player id if required {0}
                        Server.Command(string.Format(command.Text, player.UserIDString));
                        // Try to add cool down period for player-item, if successful - set state
                        if (AddPlayerCoolDownPeriodForItem(player.UserIDString, command.Text, command.CoolDownPeriodInSeconds))
                            storageHasBeenUpdated = true;
                    }
                }
                catch (Exception ex)
                {
                    if (_config.IsInDebugMode)
                        Puts(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Adds inventory items to a player
        /// </summary>
        /// <param name="player">The player to add inventory items to</param>
        /// <param name="inventoryItems">The inventory items to add</param>
        private void AddInventoryItems(BasePlayer player, List<InventoryItem> inventoryItems, out bool storageHasBeenUpdated)
        {
            // Default update state
            storageHasBeenUpdated = false;

            // Add items to inventory
            foreach (var inventoryItem in inventoryItems)
            {
                try
                {
                    // Check that player isnt in the cool down period for current item
                    if (!IsPlayerInCoolDownPeriodForItem(player.UserIDString, inventoryItem.ShortCode))
                    {
                        // If the player is not in cooldown, safely try to add inventory item
                        player.inventory.GiveItem(ItemManager.CreateByName(inventoryItem.ShortCode, NumberHelper.GenerateRandomNumber(inventoryItem.MinAmount, inventoryItem.MaxAmount))); //FinalizedInventoryItem

                        // Try to add cool down period for player-item, if successful - set state
                        if (AddPlayerCoolDownPeriodForItem(player.UserIDString, inventoryItem.ShortCode, inventoryItem.CoolDownPeriodInSeconds) == true)
                            storageHasBeenUpdated = true;
                    }
                }
                catch (Exception ex)
                {
                    if (_config.IsInDebugMode)
                        Puts(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Checks if a user has a cool down end date cached and if so, that the cool down period is still in effect (or not)
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="item">The item to check cooldown period for</param>
        /// <returns>If the player is in the cool down period for item</returns>
        private bool IsPlayerInCoolDownPeriodForItem(string userId, string item)
        {
            // Create key and default cooldown
            var key = $"{userId}_{item}";
            DateTime coolDownEndDate = DateTime.MinValue;

            // Look for cooldown period matching key
            if (_storedData.PlayerItemCooldownPeriods.TryGetValue(key, out coolDownEndDate))
            {
                // Check that the cooldown period hasnt ended - ends here if it hasnt
                if (coolDownEndDate > DateTime.UtcNow)
                    return true;

                // If we have past the cooldown period, we can then remove that old item
                _storedData.PlayerItemCooldownPeriods.Remove(key);
            }

            // Player has no restriction (cooldown period) for item sort code
            return false;
        }

        /// <summary>
        /// Adds a cool down period for a player-item
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="item">The item to check cooldown period for</param>
        private bool AddPlayerCoolDownPeriodForItem(string userId, string item, int coolDownPeriodInSeconds)
        {
            // If set to 0 or less, no point persisting
            if (coolDownPeriodInSeconds <= 0)
                return false;

            // Create key
            var key = $"{userId}_{item}";

            // Add cool down period (date + cool down in seconds) locally (in-memory)
            _storedData.PlayerItemCooldownPeriods?.Add(key, DateTime.UtcNow.AddSeconds(coolDownPeriodInSeconds));

            return true;
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
                _config = Config.ReadObject<StarterKitConfig>();

                // If empty then load the default
                if (_config == null)
                {
                    // Create new config with defaults
                    _config = new StarterKitConfig();

                    // Save the config
                    Config.WriteObject(_config);
                }
            }
            catch (Exception ex)
            {
                if (_config.IsInDebugMode)
                    Puts(ex.StackTrace);
            }
        }

        #endregion Helpers
    }

    #region Helper Classes

    #region Services

    public class KitBuilder
    {
        /// <summary>
        /// Builds and returns a kit based on runtype
        /// </summary>
        /// <param name="userId">The user kit is for</param>
        /// <param name="runType">The desired run type</param>
        /// <param name="kits">The kits to build and select from</param>
        /// <param name="currentUserKits">The current user kits (applicable for runtypes acending/decending)</param>
        /// <returns>A kit to add to user</returns>
        public static Kit SelectAndBuildKit(string userId, RunType runType, List<Kit> kits, IDictionary<string, string> currentUserKits = null)
        {
            // Stop now if no kits to choose from
            if (!kits?.Any() ?? false)
                return null;

            // Select and build kit
            switch (runType)
            {
                case RunType.Acending:
                    if (currentUserKits == null)
                        return null;
                    return ChooseAndBuildKitInAcendingOrder(userId, currentUserKits, kits);
                case RunType.Decending:
                    return ChooseAndBuildKitInDecendingOrder(kits);
                case RunType.Random:
                    return ChooseAndBuildKitInRandomOrder(kits);
            };

            return null;
        }

        /// <summary>
        /// Select kit in acending order and save it in memory
        /// </summary>
        /// <param name="userId">Player requesting</param>
        /// <param name="currentUserKits">In memory user kits</param>
        /// <param name="kits">kits to select from</param>
        /// <returns>Selected and built kit</returns>
        private static Kit ChooseAndBuildKitInAcendingOrder(string userId, IDictionary<string, string> currentUserKits, List<Kit> kits)
        {
            // Basic checks
            if (!kits?.Any() ?? false)
                return null;
            else if (kits.Count == 1)
                return BuildKit(userId, kits.FirstOrDefault(), currentUserKits);

            // Try find existing
            var success = currentUserKits.TryGetValue($"{userId}", out var currentPlayerKit);
            if (success)
            {
                // Find index of current item
                var nextIndex = kits.FindIndex(x => x.Name == currentPlayerKit) + 1;

                // Check index + 1 doesn't exceed the list length, if so then start again (1st item)
                if (nextIndex != kits.Count)
                    return BuildKit(userId, kits[nextIndex], currentUserKits);
            }

            // If user doesnt have current player kit listed, add it
            return BuildKit(userId, kits.FirstOrDefault(), currentUserKits);
        }

        /// <summary>
        /// Build a kit from definition
        /// </summary>
        /// <param name="userId">Player</param>
        /// <param name="kit">The kit to build (finalize)</param>
        /// <param name="currentUserKits">Currently registered user kits</param>
        /// <returns></returns>
        private static Kit BuildKit(string userId, Kit kit, IDictionary<string, string> currentUserKits)
        {
            // Set as selected in memory
            currentUserKits[userId] = kit.Name;

            kit.ItemsToGiveRandomly

            return kit;
        }
    }

    /// <summary>
    /// The interface to ensure weight is implemented
    /// </summary>
    public interface IWeightedItem
    {
        /// <summary>
        /// The weight of the item
        /// </summary>
        decimal Weight { get; set; }
    }

    public class NumberHelper
    {
        public static int GenerateRandomNumber(int min, int max)
        {
            return Convert.ToInt32(Math.Round(Convert.ToDouble(UnityEngine.Random.Range(Convert.ToSingle(min), Convert.ToSingle(max)))));
        }

        /// <summary>
        /// Takes a list of items (must implement IWeightedItem to have weight property) and selects one randomly taking weight into consideration
        /// </summary>
        /// <typeparam name="T">The type of item to random select</typeparam>
        /// <param name="items">The items to select from</param>
        /// <returns>A randomly selected item (by weight)</returns>
        public static T RunWeightedRandomSelection<T>(List<T> items)
            where T : class, IWeightedItem
        {
            var totalProbability = items.Sum(x => x.Weight);
            var distribution = GenerateCumulitiveProbabilities<T>(items);

            var randomNumber = GenerateRandomNumber(0, (int)Math.Floor(totalProbability));
            return SelectChosenItem<T>(distribution, randomNumber);
        }

        /// <summary>
        /// Uses a set of
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T SelectChosenItem<T>(List<(T, decimal)> items, int value)
           where T : class, IWeightedItem
        {
            foreach ((var item, var cumulitiveProbability) in items)
            {
                if (cumulitiveProbability >= (decimal)value)
                    return item;
            }

            return null;
        }

        /// <summary>
        /// Generated cumulitive probabilities
        /// </summary>
        /// <typeparam name="T">The type of items (must have weight - IWeightedItem)</typeparam>
        /// <param name="items">The items</param>
        /// <returns>List of cumulitive probabilities for items</returns>
        public static List<(T, decimal)> GenerateCumulitiveProbabilities<T>(List<T> items)
            where T : IWeightedItem
        {
            var weightedItems = new List<(T, decimal)>();
            var cumuluitiveProbability = 0.0m;

            foreach (var item in items)
            {
                cumuluitiveProbability += item.Weight;
                weightedItems.Add((item, cumuluitiveProbability));
            }

            return weightedItems;
        }
    }

    public class StoredDataService
    {
        /// <summary>
        /// Data storage file name
        /// </summary>
        private const string _startKitDataStorageKey = "StarterKitData";

        /// <summary>
        /// Loads stored data
        /// </summary>
        public static StoredData LoadStoredData()
        {
            // Load stored data from file
            var savedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>(_startKitDataStorageKey);

            // If nothing already exists, generate default properties (init list/s and so forth)
            if (savedData?.PlayerItemCooldownPeriods == null)
                savedData = new StoredData().GenerateDefaults();

            return savedData;
        }

        /// <summary>
        /// Save the stored data
        /// </summary>
        public static StoredData ResetStoredData()
        {
            // Create new storage data object
            var storedData = new StoredData().GenerateDefaults();

            // Overwrite saved data
            Interface.Oxide.DataFileSystem.WriteObject(_startKitDataStorageKey, storedData);

            return storedData;
        }

        /// <summary>
        /// Persist stored data to file
        /// </summary>
        /// <param name="storedData">The data to store</param>
        public static void SaveStoredData(StoredData storedData)
        {
            Interface.Oxide.DataFileSystem.WriteObject(_startKitDataStorageKey, storedData);
        }
    }

    #endregion

    #region Enum

    /// <summary>
    /// Map to control way kits are given in (this is only applicable if more than one kit has been added to "Kits")
    /// </summary>
    public enum RunType
    {
        Random = 0,
        Acending = 1,
        Decending = 2
    }

    #endregion

    #region Models

    /// <summary>
    /// Config class
    /// </summary>
    public class StarterKitConfig
    {
        /// <summary>
        /// If the plugin is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// To allow stacktraces to print to console
        /// </summary>
        public bool IsInDebugMode { get; set; }

        /// <summary>
        /// Map to control way kits are given in (this is only applicable if more than one kit has been added to "Kits")
        /// </summary>
        public RunType RunType { get; set; }

        /// <summary>
        /// Allows admin mapping of users-kits so that specific kits can be given (controls availability of console command)
        /// </summary>
        public bool EnableConsole { get; set; }

        /// <summary>
        /// All assignable kits
        /// </summary>
        public List<Kit> Kits { get; set; }

        /// <summary>
        /// Constructor that defaults config variables
        /// </summary>
        public StarterKitConfig()
        {
            RunType = RunType.Acending;
            Kits = new List<Kit>();
        }
    }

    /// <summary>
    /// A set of kit to give
    /// </summary>
    public class Kit : IWeightedItem
    {
        /// <summary>
        /// If the start kit is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The name of the kit
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Allows for the commands to be turned of (if not needed) for on respawn command actions
        /// </summary>
        public bool CommandsToRunOnRespawnEnabled { get; set; }

        /// <summary>
        /// List of commands to run as part of starter kit (ie. can give recycler) on respawn
        /// </summary>
        public List<Command> CommandsToRunOnRespawn { get; set; }

        /// <summary>
        /// Allows items to be added to player inventory on respawn to be turned of (if not needed)
        /// </summary>
        public bool ItemsToAddToInventoryOnRespawnEnabled { get; set; }

        /// <summary>
        /// Items to add to the inventory on respawn
        /// </summary>
        public List<InventoryItem> ItemsToAddToInventoryOnRespawn { get; set; }

        /// <summary>
        /// Switch to enable the respawn message to be shown
        /// </summary>
        public bool ShowRespawnMessage { get; set; }

        /// <summary>
        /// The message to be shown on respawn (if enabled) to alert user to starter kit
        /// </summary>
        public string RespawnMessage { get; set; }

        /// <summary>
        /// Randomly selected items used if "random" is supplied as shortcode
        /// </summary>
        public RandomGiveAwayItems ItemsToGiveRandomly { get; set; }

        /// <summary>
        /// Switch to remove all existing inventory items (stone & torch)
        /// </summary>
        public bool RemoveExistingInventory { get; set; }

        /// <summary>
        /// The prob weight for kit to be selected as
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Constructor that defaults kit variables
        /// </summary>
        public Kit()
        {

        }
    }

    /// <summary>
    /// Give away items
    /// </summary>
    public class RandomGiveAwayItems
    {
        /// <summary>
        /// Items list to select from
        /// </summary>
        public List<RandomInventoryItem> Items { get; set; }

        /// <summary>
        /// Minimum amount selected
        /// </summary>
        public int MinAmount { get; set; }

        /// <summary>
        /// Maximum amount selected
        /// </summary>
        public int MaxAmount { get; set; }

        /// <summary>
        /// Allow duplicates
        /// </summary>
        public bool CanHaveDuplicates { get; set; }
    }

    /// <summary>
    /// Holds the shortcode which defines an inventory item (basic)
    /// </summary>
    public class BasicInventoryItem
    {
        /// <summary>
        /// The item shortcode eg. ammo.shotgun.fire
        /// </summary>
        public string ShortCode { get; set; }
    }

    /// <summary>
    /// An inventory item to give
    /// </summary>
    public class InventoryItem : BasicInventoryItem
    {
        /// <summary>
        /// The minimum number of shortcode item to give
        /// </summary>
        public int MinAmount { get; set; }

        /// <summary>
        /// The minimum number of shortcode item to give
        /// </summary>
        public int MaxAmount { get; set; }

        /// <summary>
        /// How long until item can once be given on respawn
        /// </summary>
        public int CoolDownPeriodInSeconds { get; set; }

        /// <summary>
        /// If item can have attachments, this is where they can be set - seqiential priority
        /// </summary>
        public List<BasicInventoryItem> Attachments { get; set; }
    }

    /// <summary>
    /// An inventory item to give
    /// </summary>
    public class RandomInventoryItem : InventoryItem, IWeightedItem
    {
        /// <summary>
        /// The prob weight for item to be selected as
        /// </summary>
        public decimal Weight { get; set; }
    }

    public class FinalizedInventoryItem : BasicInventoryItem
    {
        /// <summary>
        /// The number of shortcode item to give
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// How long until item can once be given on respawn
        /// </summary>
        public int CoolDownPeriodInSeconds { get; set; }

        /// <summary>
        /// If item can have attachments, this is where they can be set - seqiential priority
        /// </summary>
        public List<BasicInventoryItem> Attachments { get; set; }

        /// <summary>
        /// The prob weight for item to be selected as
        /// </summary>
        public decimal Weight { get; set; }
    }

    /// <summary>
    /// Model to hold data for storage
    /// </summary>
    public class StoredData
    {
        /// <summary>
        /// Player-item cool down periods
        /// </summary>
        public Dictionary<string, DateTime> PlayerItemCooldownPeriods { get; set; }

        /// <summary>
        /// Generate default property values
        /// </summary>
        public StoredData GenerateDefaults()
        {
            PlayerItemCooldownPeriods = new Dictionary<string, DateTime>();
            return this;
        }
    }

    /// <summary>
    /// A command to execute
    /// </summary>
    public class Command
    {
        /// <summary>
        /// The command text to execute
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// How long until item can once be given on respawn
        /// </summary>
        public int CoolDownPeriodInSeconds { get; set; }
    }

    #endregion

    #endregion Helper Classes
}
