using BepInEx.Logging;
using OutwardModsCommunicator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OutwardModsCommunicator.Managers
{
    public static class LogManager
    {
        public static string prefix = "[GymMed-Mods-Communicator]";

        internal static ManualLogSource? Logger;

        public static void InitializeLogger(ManualLogSource logger)
        {
            Logger = logger;
        }

        public static void Log(string message, ENUM_LOG_LEVELS logType = ENUM_LOG_LEVELS.Info)
        {
            switch(logType)
            {
                case ENUM_LOG_LEVELS.Info:
                    {
                        LogInfo(message); 
                        break;
                    }
                case ENUM_LOG_LEVELS.Warning:
                    {
                        LogWarning(message); 
                        break;
                    }
                case ENUM_LOG_LEVELS.Error:
                    {
                        LogError(message); 
                        break;
                    }
                default:
                    {
                        LogInfo(message); 
                        break;
                    }
            }
        }

        public static void LogInfo(string message)
        {
            if (Logger != null)
                LogManager.Logger.LogInfo(prefix + " " + message);
            else
                Console.WriteLine(prefix + " " + message);
        }

        public static void LogWarning(string message)
        {
            if (Logger != null)
                Logger.LogWarning(prefix + " " + message);
            else
                Console.WriteLine(prefix + "[WARNING] " + message);
        }

        public static void LogError(string message)
        {
            if (Logger != null)
                Logger.LogError(prefix + " " + message);
            else
                Console.WriteLine(prefix + "[ERROR] " + message);
        }

    }
}
