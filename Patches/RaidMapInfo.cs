using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using EFT; 
using EFT.UI.Matchmaker;
using SPT.Reflection.Patching; 
using UnityEngine;
using BepInEx.Logging;
using System.CodeDom;
using DiscordRPC;

namespace SPTRPC
{
    public class RaidMapInfo : ModulePatch
    {
        public static ManualLogSource LogSource;
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchmakerTimeHasCome), "Show", new[] { typeof(ISession), typeof(RaidSettings) });
        }

        // Postfix method that executes after the original method
        [PatchPostfix]
        private static void Postfix(ISession session, RaidSettings raidSettings)
        {
            if (session == null || session.Profile == null || session.Profile.Info == null)
            {
                LogSource.LogInfo("No data is available :(");
                return; // Skip if data is not available
            }

            string nickname = session.Profile.Info.Nickname;

            int level = session.Profile.Info.Level;

            EPlayerSide faction = session.Profile.Info.Side;   // Convert both types to strings for use in if condition logic
            string factionString = faction.ToString();


            ESideType side = raidSettings.Side;       // Convert both types to strings for use in if condition logic
            string sideString = side.ToString();

            LocationSettingsClass.Location selectedLocation = raidSettings.SelectedLocation; // Attribute / Data type fuckery I may be stupid but this took me too long to figure out
            string locationName = selectedLocation != null ? selectedLocation.Name : "Unknown";


            switch (locationName) // Switch statement for converting map names to their actual name (for some reason BSG decided to make only 3 maps have different names)
            {
                case "ReserveBase":
                    locationName = "Reserve";
                    break;
                case "Sandbox":
                    locationName = "Ground Zero";
                    break;
                case "Laboratory":
                    locationName = "Labs";
                    break;
            };

            string imageString = locationName.ToLower();
            imageString = imageString.Replace(" ",""); // Remove capitals and spaces from name string to create a useable image string
                                                       // because Discord Application art asset names can't have either

            DateTime startTime = DateTime.UtcNow;

            if (Plugin.client != null)
            {
                if (sideString == "Savage")
                {

                    Plugin.client.SetPresence(new DiscordRPC.RichPresence()
                    {
                        Details = $"Playing as a Scav",
                        State = $"In Raid - {locationName}",
                        Timestamps = new Timestamps()
                        {
                            Start = startTime,
                            End = null              // Scav Presence
                        },
                        Assets = new Assets()
                        {
                            LargeImageKey = $"{imageString}",
                            LargeImageText = $"{locationName}",
                            SmallImageKey = null,
                            SmallImageText = null
                        }
                    });
                }
                else
                {
                    Plugin.client.SetPresence(new DiscordRPC.RichPresence()
                    {
                        Details = $"Operator {nickname}",
                        State = $"In Raid - {locationName}",
                        Timestamps = new Timestamps()
                        {
                            Start = startTime,
                            End = null              // PMC Presence
                        },
                        Assets = new Assets()
                        {
                            LargeImageKey = $"{imageString}",
                            LargeImageText = $"{locationName}",
                            SmallImageKey = $"{factionString.ToLower()}",
                            SmallImageText = $"{factionString.ToUpper()} - Lvl {level}"
                        }
                    });
                }
            }

            Plugin.firstTimeInMenu = true;
                
        }

    }
}
