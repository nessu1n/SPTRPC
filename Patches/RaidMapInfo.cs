using System;
using System.Reflection;
using DiscordRPC;
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
            // For 3.10, an extra MatchmakerPlayerControllerClass parameter was added for this method - Terkoiz
            return AccessTools.Method(typeof(MatchmakerTimeHasCome), nameof(MatchmakerTimeHasCome.Show), new[] { typeof(ISession), typeof(RaidSettings), typeof(MatchmakerPlayerControllerClass) });
        }

        // Postfix method that executes after the original method
        [PatchPostfix]
        private static void Postfix(ISession session, RaidSettings raidSettings)
        {
            // Moved this check to happen way sooner, preventing unnecessary code from running before it - Terkoiz
            if (Plugin.client == null)
            {
                return;
            }

            if (session == null || session.Profile == null || session.Profile.Info == null)
            {
                // Previous LogSource usage here was invalid and would've just produced an exception. Fixed it by pointing the call to the base plugin class LogSource - Terkoiz
                Plugin.LogSource.LogInfo("No data is available :(");
                return; // Skip if data is not available
            }

            string nickname = session.Profile.Info.Nickname;

            int level = session.Profile.Info.Level;

            EPlayerSide faction = session.Profile.Info.Side;   // Convert both types to strings for use in if condition logic
            string factionString = faction.ToString();

            ESideType side = raidSettings.Side;       // Convert both types to strings for use in if condition logic
            string sideString = side.ToString();

            LocationSettingsClass.Location selectedLocation = raidSettings.SelectedLocation; // Attribute / Data type fuckery I may be stupid but this took me too long to figure out
            string locationName = selectedLocation != null ? selectedLocation.LocalizedName : "Unknown"; // If we use the LocalizedName property, we don't need to do any conversion later :p - Terkoiz

            // Remove capitals and spaces from name string to create a useable image string
            // because Discord Application art asset names can't have either
            string imageString = locationName.ToLower().Replace(" ", ""); 

            DateTime startTime = DateTime.UtcNow;

            // Reduced the amount of duplicate code here by utilizing inline conditions for Details, SmallImageKey and SmallImageText instead of having two big "if" blocks - Terkoiz
            bool isScav = sideString == "Savage";
            Plugin.client.SetPresence(new RichPresence
            {
                Details = isScav ? "Playing as a Scav" : $"Operator {nickname}",
                State = $"In Raid - {locationName}",
                Timestamps = new Timestamps
                {
                    Start = startTime,
                    End = null
                },
                Assets = new Assets
                {
                    LargeImageKey = $"{imageString}",
                    LargeImageText = $"{locationName}",
                    SmallImageKey = isScav ? null : $"{factionString.ToLower()}",
                    SmallImageText = isScav ? null : $"{factionString.ToUpper()} - Lvl {level}"
                }
            });
        }
    }
}
