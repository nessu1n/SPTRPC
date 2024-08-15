using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using EFT;
using EFT.UI;
using SPT.Reflection.Patching;
using UnityEngine;
using BepInEx.Logging;
using DiscordRPC;
using System.Runtime.Remoting.Messaging;

namespace SPTRPC
{
    public class MenuPatch : ModulePatch
    {
        public static ManualLogSource LogSource;
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MenuScreen), "Show", new[] { typeof(Profile), typeof(MatchmakerPlayerControllerClass), typeof(ESessionMode)});
        } // Hook method that runs whenever the menu buttons are displayed

        [PatchPostfix]
        private static void Postfix(MenuScreen __instance, Profile profile, MatchmakerPlayerControllerClass matchmaker, ESessionMode sessionMode)
        {
            DateTime startTime = DateTime.UtcNow;
            if (Plugin.client != null)
            {
                if (Plugin.firstTimeInMenu)
                {
                    Plugin.client.SetPresence(new DiscordRPC.RichPresence()
                    {
                        Details = "In the menus",
                        State = "Gearing up",
                        Timestamps = new Timestamps()
                        {
                            Start = startTime,
                            End = null
                        },
                        Assets = new Assets()
                        {
                            LargeImageKey = "inthemenus"
                        }
                    });

                    Plugin.firstTimeInMenu = false;
                }
                else
                {
                    return;
                }
                
            }
        }
    }
}
