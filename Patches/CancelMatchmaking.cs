using System.Reflection;
using EFT.UI.Matchmaker;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SPTRPC.Patches
{
    public class CancelMatchmaking : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // In 4.0 and later, the method signature is AbortMatching
            return AccessTools.Method(typeof(MatchmakerTimeHasCome), nameof(MatchmakerTimeHasCome.AbortMatching)); // This runs when the cancel button is clicked in "The time has come" UI screen
        }

        [PatchPostfix]
        private static void PostFix()
        {
            Plugin.firstTimeInMenu = true;  // Reset the variable if the user decides to cancel matchmaking at the last possible chance
        }
    }
}
