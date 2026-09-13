using BepInEx;
using BepInEx.Logging;
using System;
using Discord;
using SPTRPC.Patches;

namespace SPTRPC
{
    [BepInPlugin("nessu1n.SPTRPC", "SPTRPC", "2.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;

        // This holds the main connection reference for the modern SDK
        public static Discord.Discord discordClient;
        public static Discord.ActivityManager activityManager;

        private bool isInitialized = false;
        public static bool firstTimeInMenu = true;
        public static long startTimeUnix;

        private void Awake()
        {
            LogSource = Logger;
            LogSource.LogInfo("SPTRPC Loaded, Initializing Native Social SDK...");

            if (!isInitialized)
            {
                LoadDiscordRPC();
                isInitialized = true;
            }

            new RaidMapInfo().Enable();
            new MenuPatch().Enable();
            new EndRaid().Enable();
            new CancelMatchmaking().Enable();
        }

        private void LoadDiscordRPC()
        {
            // The modern SDK expects Unix epoch timestamps rather than a raw DateTime object
            startTimeUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            LogSource.LogInfo($"Start time recorded: {startTimeUnix}");

            try
            {
                // NoRequireDiscord flag prevents the game crashing if the player doesn't have Discord running
                discordClient = new Discord.Discord(1273226966950084688, (ulong)Discord.CreateFlags.NoRequireDiscord);

                // Grab the activity sub-manager which controls the actual status texts
                activityManager = discordClient.GetActivityManager();

                // Set the custom logger callback built directly into the new SDK
                discordClient.SetLogHook(Discord.LogLevel.Info, (level, message) =>
                {
                    LogSource.LogInfo($"[Discord SDK Internal] {message}");
                });

                LogSource.LogInfo("Setting initial presence...");

                // Trigger the initial status state using the helper wrapper below
                UpdatePresence("Loading into the Menu", "", "mainmenuimage");

                LogSource.LogInfo("RPC initialized successfully!");
            }
            catch (Exception ex)
            {
                LogSource.LogError($"CRITICAL: Native Discord SDK failed to initialize! Error: {ex.Message}");
            }
        }

        // The native SDK relies on this frame-by-frame tick to fire background callbacks.
        // Without this Update loop, presence strings will never actually update in Discord
        private void Update()
        {
            if (discordClient != null)
            {
                try
                {
                    discordClient.RunCallbacks();
                }
                catch (Exception ex)
                {
                    LogSource.LogError($"Error during Discord callback tick: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Global helper method to cleanly update Rich Presence details
        /// </summary>
        public static void UpdatePresence(string state, string details, string largeImageKey)
        {
            if (activityManager == null)
            {
                LogSource.LogWarning("Cannot update presence: ActivityManager is null.");
                return;
            }

            // Create the new activity data object matching the Social SDK schema
            var activity = new Discord.Activity
            {
                State = state,
                Details = details,
                Timestamps = new Discord.ActivityTimestamps
                {
                    Start = startTimeUnix
                },
                Assets = new Discord.ActivityAssets
                {
                    LargeImage = largeImageKey,
                    LargeText = "Single Player Tarkov"
                }
            };

            // Push the update to Discord asynchronously
            activityManager.UpdateActivity(activity, (result) =>
            {
                if (result == Discord.Result.Ok)
                {
                    LogSource.LogDebug($"Presence updated: {state} | {details}");
                }
                else
                {
                    LogSource.LogWarning($"Discord status update returned an error code: {result}");
                }
            });
        }

        private void OnDestroy()
        {
            // Safely close down the Windows unmanaged memory pointer when the game terminates
            if (discordClient != null)
            {
                discordClient.Dispose();
                discordClient = null;
                activityManager = null;
                LogSource.LogInfo("Native Discord SDK client safely disposed.");
            }
        }
    }
}
