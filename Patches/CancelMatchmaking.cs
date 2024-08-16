using EFT.UI.Matchmaker;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPTRPC
{
    public class CancelMatchmaking : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MatchmakerTimeHasCome), "method_7"); // This is so incredibly jank but its an edge case fix this is the function that runs when the cancel button is clicked in "The time has come" UI screen
        }

        [PatchPostfix]
        private static void PostFix()
        {
            Plugin.firstTimeInMenu = true;  // Reset the variable if the user decides to cancel matchmaking at the last possible chance
        }
    }
}
