using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using BepInEx.Configuration;
using SkipSplashScreens.Patches;
using System.IO;

#if TDMX_IL2CPP
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
#endif

namespace SkipSplashScreens
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, ModName, MyPluginInfo.PLUGIN_VERSION)]
#if TDMX_MONO
    public class Plugin : BaseUnityPlugin
#elif TDMX_IL2CPP
    public class Plugin : BasePlugin
#endif
    {
        public const string ModName = "SkipSplashScreens";

        public static Plugin Instance;
        private Harmony _harmony;
        public new static ManualLogSource Log;

        public ConfigEntry<bool> ConfigEnabled;

#if TDMX_MONO
        private void Awake()
#elif TDMX_IL2CPP
        public override void Load()
#endif
        {
            Instance = this;

#if TDMX_MONO
            Log = Logger;
#elif TDMX_IL2CPP
            Log = base.Log;
#endif

            SetupConfig();
            SetupHarmony();
        }

        private void SetupConfig()
        {
            string dataFolder = Path.Combine("BepInEx", "data", ModName);

            ConfigEnabled = Config.Bind("General",
                "Enabled",
                true,
                "Enables the mod.");
        }

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            if (ConfigEnabled.Value)
            {
                LoadPlugin();
            }
            else
            {
                Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        public static void LoadPlugin()
        {
            bool result = true;
            // If any PatchFile fails, result will become false
            result &= Instance.PatchFile(typeof(SkipSplashScreensPatch));
            if (result)
            {
                ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
            }
            else
            {
                ModLogger.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} failed to load.", LogType.Error);
                // Unload this instance of Harmony
                // I hope this works the way I think it does
                Instance._harmony.UnpatchSelf();
            }
        }

        private bool PatchFile(Type type)
        {
            if (_harmony == null)
            {
                _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            }
            try
            {
                _harmony.PatchAll(type);
#if DEBUG
                ModLogger.Log("File patched: " + type.FullName);
#endif
                return true;
            }
            catch (Exception e)
            {
                ModLogger.Log("Failed to patch file: " + type.FullName);
                ModLogger.Log(e.Message);
                return false;
            }
        }

        // I never used these, but they may come in handy at some point
        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;

        public void StartCustomCoroutine(IEnumerator enumerator)
        {
#if TDMX_MONO
            GetMonoBehaviour().StartCoroutine(enumerator);
#elif TDMX_IL2CPP
            GetMonoBehaviour().StartCoroutine(enumerator);
#endif
        }

    }
}