using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Xml.Linq;
using OutwardModsCommunicator.Managers;
using Steamworks;
using OutwardModsCommunicator.Enums;
using System.Reflection;
using BepInEx.Configuration;
using OutwardModsCommunicator.EventBus;

namespace OutwardModsCommunicator
{
    // shortcut for OMC = OutwardModsCommunicator
    [BepInPlugin(GUID, NAME, VERSION)]
    public class OMC : BaseUnityPlugin
    {
        public const string GUID = "gymmed.outward_mods_communicator";

        public const string NAME = "Outward Mods Communicator";

        public const string VERSION = "1.0.0";

        public static string xmlFilePath { get; set; } = "";

        public static ConfigEntry<bool> EnableEventsProfiler = null!;
        public static ConfigEntry<bool> InstantLogEventsProfileData = null!;

        internal void Awake()
        {
            LogManager.InitializeLogger(this.Logger);
            #if DEBUG
            OMC.Log($"OMC@Awake called!");
            #endif

            EnableEventsProfiler = Config.Bind(
                "Event Profiler",
                "EnableEventsProfiler",
                false,
                "Enable events profiler for inspecting events timers?(You still need to call it for logging)"
            );

            InstantLogEventsProfileData = Config.Bind(
                "Event Profiler",
                "InstantLogEventsProfileData",
                false,
                "If enabled, logs each event profiling record immediately when it happens."
            );

            EventProfiler.Initialize();

            new Harmony(GUID).PatchAll();
        }

        public static string GetProjectLocation()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static void Log(string message, ENUM_LOG_LEVELS logType = ENUM_LOG_LEVELS.Info)
        {
            LogManager.Log(message, logType);
        }

        [HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
        public class ResourcesPrefabManager_Load
        {
            static void Postfix(ResourcesPrefabManager __instance)
            {
                #if DEBUG
                OMC.Log($"OMC@ResourcesPrefabManager_Load:\"{xmlFilePath}\"");
                #endif

                if (string.IsNullOrEmpty(xmlFilePath))
                    return;

                ConfigOverrideManager.OverrideConfigsFromFile(xmlFilePath);
                ConfigOverrideManager.OverrideConfigsFromFile(Path.Combine(GetProjectLocation(), "PlayerModsOverrides.xml"));
            }
        }
    }
}
