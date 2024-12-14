using System;
using System.Reflection;
using DiscordRPC;
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
            // I rearranged the logic here to reduce nesting - Terkoiz
            if (Plugin.client == null)
            {
                return;
            }

            if (!Plugin.firstTimeInMenu)
            {
                return;
            }

            Plugin.client.SetPresence(new RichPresence
            {
                Details = "In the menus",
                State = "Gearing up",
                Timestamps = new Timestamps
                {
                    Start = DateTime.UtcNow,
                    End = null
                },
                Assets = new Assets
                {
                    LargeImageKey = "inthemenus"
                }
            });

            Plugin.firstTimeInMenu = false;
        }
    }
}
