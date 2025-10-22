using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using BepInEx;
using BepInEx.Bootstrap;
using System.IO;

namespace OutwardModsCommunicator.Managers
{
    public class ConfigOverrideManager
    {
        public static void ChangeOriginalConfigs(ConfigOverrides overrides)
        {
            foreach (var mod in overrides.Mods)
            {
                ChangeOriginalModConfig(mod);
            }
        }

        public static void ChangeOriginalModConfig(ModOverride mod)
        {
            string cfgPath = Path.Combine(Paths.ConfigPath, $"{mod.ModGUID}.cfg");
            if (!File.Exists(cfgPath))
            {
                OMC.Log($"Skipping {mod.ModGUID} — config file not found.");
                return;
            }

            var cfg = new ConfigFile(cfgPath, true);

            foreach (var section in mod.Sections)
            {
                foreach (var entry in section.Entries)
                {
                    try
                    {
                        var configEntry = cfg[section.Name, entry.Key];
                        if (configEntry == null)
                        {
                            OMC.Log($"Missing key {section.Name}:{entry.Key} in {mod.ModGUID}, skipping.");
                            continue;
                        }

                        // Convert string to target type safely
                        object convertedValue = Convert.ChangeType(entry.Value, configEntry.SettingType);
                        configEntry.BoxedValue = convertedValue;
                        OMC.Log($"Updated {mod.ModGUID}:{section.Name}:{entry.Key} = {entry.Value}");
                    }
                    catch (Exception ex)
                    {
                        OMC.Log($"Error updating {mod.ModGUID}:{section.Name}:{entry.Key}: {ex.Message}");
                    }
                }
            }

            cfg.Save();
        }

        public static void OverrideConfigsFromFile(string filePath =  "MyModpackOverrides.xml")
        {
            #if DEBUG
            OMC.Log($"ConfigOverrideManager@OverrideConfigsFromFile: filePath:\"{filePath}\"");
            #endif
            var overrides = ConfigProfileLoader.LoadFromXml(
                filePath
            );

            if (overrides == null)
            {
                OMC.Log($"Couldn't open \"{filePath}\". You are sure that provided path is correct?", Enums.ENUM_LOG_LEVELS.Error);
                return;
            }

            ConfigOverrideManager.OverrideConfigsValues(overrides);
        }

        // =====================================================
        // =============== PUBLIC ENTRY POINT ==================
        // =====================================================
        public static void OverrideConfigsValues(ConfigOverrides overrides)
        {
            try
            {
                #if DEBUG
                OMC.Log("Overrides count:" + overrides.Mods.Count);
                #endif
                if (overrides?.Mods == null || overrides.Mods.Count == 0)
                {
                    OMC.Log("No overrides provided.");
                    return;
                }

                foreach (var mod in overrides.Mods)
                    ApplyOverridesToMod(mod);
            }
            catch(Exception e)
            {
                OMC.Log($"ConfigOverrideManager@OverrideConfigsValues:{e.Message}", Enums.ENUM_LOG_LEVELS.Error);
                return;
            }
        }

        // =====================================================
        // =============== MOD-LEVEL HANDLER ===================
        // =====================================================
        private static void ApplyOverridesToMod(ModOverride mod)
        {
            if (string.IsNullOrWhiteSpace(mod.ModGUID))
            {
                OMC.Log("Skipping mod with empty GUID.", Enums.ENUM_LOG_LEVELS.Warning);
                return;
            }

            if (!Chainloader.PluginInfos.TryGetValue(mod.ModGUID, out var plugin))
            {
                OMC.Log($"Plugin not found: {mod.ModGUID}", Enums.ENUM_LOG_LEVELS.Warning);
                return;
            }

            var configFile = plugin.Instance?.Config;
            if (configFile == null)
            {
                OMC.Log($"Plugin has no ConfigFile: {mod.ModGUID}", Enums.ENUM_LOG_LEVELS.Warning);
                return;
            }

            if (mod.Sections == null || mod.Sections.Count == 0)
            {
                OMC.Log($"No sections for {mod.ModGUID} — nothing to apply.");
                return;
            }

            #if DEBUG
            OMC.Log($"ConfigOverrideManager@ApplyOverridesToMod mod:\"{mod.ModGUID}\" total sections:\"{mod.Sections.Count}\"");
            #endif
            foreach (var section in mod.Sections)
                ApplyOverridesToSection(mod.ModGUID, configFile, section);
        }

        // =====================================================
        // ============= SECTION-LEVEL HANDLER =================
        // =====================================================
        private static void ApplyOverridesToSection(string modGUID, ConfigFile configFile, SectionOverride section)
        {
            if (string.IsNullOrWhiteSpace(section.Name))
            {
                OMC.Log($"Sections without name in {modGUID}", Enums.ENUM_LOG_LEVELS.Warning);
                return;
            }

            if (section.Entries == null || section.Entries.Count == 0)
            {
                OMC.Log($"Sections without entries in {modGUID}", Enums.ENUM_LOG_LEVELS.Warning);
                return;
            }

            #if DEBUG
            OMC.Log($"ConfigOverrideManager@ApplyOverridesToSection mod:\"{modGUID}\" section:\"{section.Name}\" Total entries:\"{section.Entries.Count}\"");
            #endif
            foreach (var entry in section.Entries)
                ApplyOverrideToEntry(modGUID, configFile, section.Name, entry);
        }

        // =====================================================
        // =============== ENTRY-LEVEL HANDLER =================
        // =====================================================
        private static void ApplyOverrideToEntry(string modGUID, ConfigFile configFile, string sectionName, EntryOverride entry)
        {
            if (string.IsNullOrWhiteSpace(entry.Key))
            {
                OMC.Log($"Mod:\"{modGUID}\" Section: \"{sectionName}\" has empty entry key", Enums.ENUM_LOG_LEVELS.Warning);
                return;
            }

            var configEntryObj = FindConfigEntry(configFile, sectionName, entry.Key);
            if (configEntryObj == null)
            {
                OMC.Log($"Config entry not found: {modGUID} {sectionName}:{entry.Key}", Enums.ENUM_LOG_LEVELS.Warning);
                return;
            }

            Type settingType = GetSettingType(configEntryObj);
            if (settingType == null)
                settingType = typeof(string);

            if (!TryConvertValue(entry.Value, settingType, out var convertedValue))
            {
                OMC.Log($"Failed to convert '{entry.Value}' to {settingType.Name} for {modGUID}:{sectionName}:{entry.Key}", Enums.ENUM_LOG_LEVELS.Warning);
                return;
            }

            if (SetBoxedValue(configEntryObj, convertedValue))
                OMC.Log($"Applied {modGUID} {sectionName}:{entry.Key} = {entry.Value}");
            else
                OMC.Log($"Cannot set BoxedValue for {modGUID}:{sectionName}:{entry.Key}", Enums.ENUM_LOG_LEVELS.Warning);
        }

        // =====================================================
        // =============== HELPER METHODS ======================
        // =====================================================
        private static object? FindConfigEntry(ConfigFile configFile, string sectionName, string key)
        {
            try
            {
                // Try direct indexer first
                return configFile[sectionName, key];
            }
            catch
            {
                // Fallback to search
                try
                {
                    var valuesProp = configFile.GetType().GetProperty("Values", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var values = valuesProp?.GetValue(configFile) as System.Collections.IEnumerable;

                    if (values == null) return null;

                    foreach (var v in values)
                    {
                        var def = v.GetType().GetProperty("Definition")?.GetValue(v);
                        if (def == null) continue;

                        var defKey = def.GetType().GetProperty("Key")?.GetValue(def) as string;
                        var defSection = def.GetType().GetProperty("Section")?.GetValue(def) as string;

                        if (defKey == key && defSection == sectionName)
                            return v;
                    }
                }
                catch { }
            }
            return null;
        }

        private static Type GetSettingType(object configEntryObj)
        {
            try
            {
                var prop = configEntryObj.GetType().GetProperty("SettingType");
                if (prop != null)
                {
                    var type = prop.GetValue(configEntryObj) as Type;
                    if (type != null) return type;
                }

                // fallback
                var boxed = configEntryObj.GetType().GetProperty("BoxedValue")?.GetValue(configEntryObj);
                return boxed?.GetType() ?? typeof(string);
            }
            catch
            {
                return typeof(string);
            }
        }

        private static bool TryConvertValue(string value, Type targetType, out object? result)
        {
            try
            {
                if (targetType.IsEnum)
                {
                    result = Enum.Parse(targetType, value, ignoreCase: true);
                }
                else
                {
                    result = Convert.ChangeType(value, targetType);
                }
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private static bool SetBoxedValue(object configEntryObj, object? value)
        {
            try
            {
                var prop = configEntryObj.GetType().GetProperty("BoxedValue", BindingFlags.Instance | BindingFlags.Public);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(configEntryObj, value);
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}
