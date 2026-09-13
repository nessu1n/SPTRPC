using System;
using System.Reflection;
using Discord;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SPTRPC.Patches
{
    public class MenuPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // For this patch, it doesn't matter which Show method is hooked onto, so I simplified the logic here - Terkoiz
            // Hook method that runs whenever the menu buttons are displayed
            return AccessTools.FirstMethod(typeof(MenuScreen), method => method.Name == nameof(MenuScreen.Show));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            // Check against our new native client setup
            if (Plugin.discordClient == null || Plugin.activityManager == null)
            {
                return;
            }

            if (!Plugin.firstTimeInMenu)
            {
                return;
            }

            // Record the current time in Unix format for the "Time Elapsed" clock
            long currentMenuStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Construct the updated activity profile directly matching the GameSDK schema
            var menuActivity = new Discord.Activity
            {
                Details = "In the menus",
                State = "Gearing up",
                Timestamps = new Discord.ActivityTimestamps
                {
                    Start = currentMenuStartTime
                },
                Assets = new Discord.ActivityAssets
                {
                    LargeImage = "inthemenus",
                    LargeText = "Main Menu"
                }
            };

            // Push the data directly to Discord's unmanaged memory manager
            Plugin.activityManager.UpdateActivity(menuActivity, (result) =>
            {
                if (result == Discord.Result.Ok)
                {
                    Plugin.LogSource.LogDebug("Successfully updated Menu Presence!");
                }
            });

            Plugin.firstTimeInMenu = false;
        }
    }
}
