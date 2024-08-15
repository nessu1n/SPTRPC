using EFT.UI;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EFT.UI.SessionEnd;

namespace SPTRPC.Patches
{
    public class EndRaid : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SessionResultExitStatus), "Show", new[]{typeof(Profile),typeof(PlayerVisualRepresentation),typeof(ESideType),typeof(ExitStatus),typeof(TimeSpan),typeof(ISession),typeof(bool)});
        }

        [PatchPostfix]
        private static void PostFix(Profile activeProfile, PlayerVisualRepresentation lastPlayerState, ESideType side, ExitStatus exitStatus, TimeSpan raidTime, ISession session, bool isOnline)
        {
            Plugin.firstTimeInMenu = true; // Literally just need to hook this to only allow the menu status to be displayed when in the actual menu
        }
    }
}
