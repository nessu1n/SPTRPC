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
            // In 3.10, the target method changed from method_7 to method_9 - Terkoiz
            return AccessTools.Method(typeof(MatchmakerTimeHasCome), nameof(MatchmakerTimeHasCome.method_9)); // This is so incredibly jank but its an edge case fix this is the function that runs when the cancel button is clicked in "The time has come" UI screen
        }

        [PatchPostfix]
        private static void PostFix()
        {
            Plugin.firstTimeInMenu = true;  // Reset the variable if the user decides to cancel matchmaking at the last possible chance
        }
    }
}
