using System;
using System.Reflection;
using Discord;
using EFT;
using EFT.UI.Matchmaker;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SPTRPC.Patches
{
    public class RaidMapInfo : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // For 4.0.0 and later, the method signature has changed to include MatchmakerPlayersController
            return AccessTools.Method(typeof(MatchmakerTimeHasCome), nameof(MatchmakerTimeHasCome.Show), new[] { typeof(IEftSession), typeof(RaidSettings), typeof(MatchmakerPlayersController) });
        }

        // Postfix method that executes after the original method
        [PatchPostfix]
        private static void Postfix(IEftSession session, RaidSettings raidSettings)
        {
            // Check against our new native client setup
            if (Plugin.discordClient == null || Plugin.activityManager == null)
            {
                return;
            }

            if (session == null || session.Profile == null || session.Profile.Info == null)
            {
                Plugin.LogSource.LogInfo("No data is available :(");
                return; // Skip if data is not available
            }

            string nickname = session.Profile.Info.Nickname;
            int level = session.Profile.Info.Level;

            EPlayerSide faction = session.Profile.Info.Side;
            string factionString = faction.ToString();

            ESideType side = raidSettings.Side;
            string sideString = side.ToString();

            JsonType.LocationSettings.Location selectedLocation = raidSettings.SelectedLocation;
            string locationName = selectedLocation != null ? selectedLocation.LocalizedName : "Unknown";

            // Clean string formatting for Discord asset requirements
            string imageString = locationName.ToLower().Replace(" ", "");

            // Record the exact time the raid screen loaded using Unix format
            long currentRaidStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Set up conditions for Scav gameplay parsing
            bool isScav = sideString == "Savage";

            // Construct the updated activity profile directly matching the GameSDK schema
            var raidActivity = new Discord.Activity
            {
                Details = isScav ? "Playing as a Scav" : $"Operator {nickname}",
                State = $"In Raid - {locationName}",
                Timestamps = new Discord.ActivityTimestamps
                {
                    Start = currentRaidStartTime
                },
                Assets = new Discord.ActivityAssets
                {
                    LargeImage = imageString,
                    LargeText = locationName,
                    // If playing as a Scav, drop the small badge properties entirely
                    SmallImage = isScav ? null : factionString.ToLower(),
                    SmallText = isScav ? null : $"{factionString.ToUpper()} - Lvl {level}"
                }
            };

            // Push the data directly to Discord's unmanaged memory manager
            Plugin.activityManager.UpdateActivity(raidActivity, (result) =>
            {
                if (result == Discord.Result.Ok)
                {
                    Plugin.LogSource.LogDebug($"Successfully pushed Raid Presence for {locationName}!");
                }
                else
                {
                    Plugin.LogSource.LogWarning($"Failed to push raid status update. Discord returned: {result}");
                }
            });
        }
    }
}
