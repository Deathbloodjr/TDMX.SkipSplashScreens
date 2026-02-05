using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkipSplashScreens.Patches
{
    internal class SkipSplashScreensPatch
    {
        [HarmonyPatch(typeof(BootManager))]
        [HarmonyPatch(nameof(BootManager.Initialize))]
        [HarmonyPostfix]
#if TDMX_MONO
        private static void BootManager_Initialize_Postfix(BootManager __instance, ref BootManager.BootState ___m_stateBoot)
#else
        private static void BootManager_Initialize_Postfix(BootManager __instance)
#endif
        {
#if TDMX_MONO
            ___m_stateBoot = BootManager.BootState.End;
#else
            BootManager.m_stateBoot = BootManager.BootState.End;
#endif
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.ChangeScene("Title", false);
        }
    }
}
