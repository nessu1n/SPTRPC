using System.Reflection;
using EFT.UI.SessionEnd;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SPTRPC.Patches
{
    public class EndRaid : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // For this patch, it doesn't matter which Show method is hooked onto, so I simplified the logic here - Terkoiz
            return AccessTools.FirstMethod(typeof(SessionResultExitStatus), method => method.Name == nameof(SessionResultExitStatus.Show));
        }

        [PatchPostfix]
        private static void PostFix() // All the parameters in this postfix patch were unused, so I removed them - Terkoiz
        {
            Plugin.firstTimeInMenu = true; // Literally just need to hook this to only allow the menu status to be displayed when in the actual menu
        }
    }
}
