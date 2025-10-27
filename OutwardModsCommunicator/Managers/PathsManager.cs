using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardModsCommunicator.Managers
{
    public static class PathsManager
    {
        public static string ConfigDirectoryName = "gymmed.Mods_Communicator";
        public static string ConfigPath = Path.Combine(Paths.ConfigPath, ConfigDirectoryName);
        public static string PlayerOverrideFileName = "PlayerModsOverrides.xml";

        public static string GetPlayerOverrideLocation()
        {
            return Path.Combine(ConfigPath, PlayerOverrideFileName);
        }
    }
}
