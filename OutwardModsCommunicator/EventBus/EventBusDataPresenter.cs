using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardModsCommunicator.EventBus
{
    public class EventBusDataPresenter
    {
        /// <summary>
        /// Log all mods’ registered publishers info as a single huge data block.
        /// </summary>
        public static void LogRegisteredEvents()
        {
            var regs = EventBus.GetRegisteredEvents();
            OMC.Log("==== EventBus Registered Events (ALL MODS) ====");

            foreach (var modPair in regs)
                LogRegisteredEventsForModInternal(modPair.Key, modPair.Value);

            OMC.Log("==== End of Registered Events ====");
        }

        /// <summary>
        /// Log a specific mod’s registered publishers info as a single data block.
        /// </summary>
        public static void LogModRegisteredEvents(string modNamespace)
        {
            var regs = EventBus.GetRegisteredEvents();

            if (!regs.TryGetValue(modNamespace, out var modEvents))
            {
                OMC.Log($"No registered events found for mod '{modNamespace}'");
                return;
            }

            LogRegisteredEventsForModInternal(modNamespace, modEvents);
        }

        /// <summary>
        /// Internal reusable function for logging registered events of one mod.
        /// </summary>
        private static void LogRegisteredEventsForModInternal(string modNamespace, Dictionary<string, EventSchema> modEvents)
        {
            OMC.Log($"==== EventBus Registered Events for Mod '{modNamespace}' ====");

            foreach (var evtPair in modEvents)
            {
                string eventName = evtPair.Key;
                EventSchema schema = evtPair.Value;

                OMC.Log($"  Event '{eventName}' fields:");

                if (schema.Fields.Count == 0)
                {
                    OMC.Log("    (no fields declared)");
                    continue;
                }

                foreach (var field in schema.Fields)
                {
                    string fieldName = field.Key;
                    string typeName = field.Value?.Name ?? "null";
                    OMC.Log($"    Parameter {fieldName} : Type {typeName}");
                }
            }

            OMC.Log($"==== End of Registered Events for Mod '{modNamespace}' ====");
        }

        /// <summary>
        /// Log all publishers(already executed) info as a single huge data block.
        /// </summary>
        public static void LogPublishers()
        {
            var published = EventBus.GetModPublishedPayloads();

            OMC.Log("==== EventBus Publishers ====");
            foreach (var modPair in published)
            {
                LogPublishersEvents(modPair.Key, modPair.Value);
            }
            OMC.Log("==== End of Publishers ====");
        }

        /// <summary>
        /// Log publishers(already executed) info for a specific mod as a single huge data block.
        /// </summary>
        public static void LogModPublishers(string modNamespace)
        {
            var published = EventBus.GetModPublishedPayloads();

            if (!published.TryGetValue(modNamespace, out var modEvents))
            {
                OMC.Log($"No published events found for mod '{modNamespace}'");
                return;
            }

            LogPublishersEvents(modNamespace, modEvents);
        }

        /// <summary>
        /// Log specific mod publisher events as a single huge data block.
        /// </summary>
        public static void LogPublishersEvents(string modNamespace, Dictionary<string, EventPayload>modEvents)
        {
            OMC.Log($"==== EventBus Publishers for Mod '{modNamespace}' ====");

            foreach (var evtPair in modEvents)
            {
                LogPublishersEventsVariables(evtPair.Key, evtPair.Value);
            }

            OMC.Log($"==== End of Publishers for Mod '{modNamespace}' ====");
        }

        /// <summary>
        /// Log specific mod publisher event variables as a single huge data block.
        /// </summary>
        public static void LogPublishersEventsVariables(string eventName, EventPayload payload)
        {

            OMC.Log($"  Event '{eventName}' publishes keys:");

            foreach (var kv in payload)
            {
                string keyName = kv.Key;
                string typeName = kv.Value?.GetType().Name ?? "null";
                string valueStr = kv.Value?.ToString() ?? "null";
                OMC.Log($"    {keyName} : {typeName} | Value={valueStr}");
            }
        }

        /// <summary>
        /// Logs all mods, their events, and subscriber info as a single huge data block.
        /// </summary>
        public static void LogAllModsSubsribers()
        {
            var modSubscribers = EventBus.GetModSubscribers();

            OMC.Log("==== EventBus Data: ALL MODS ====");

            foreach (var modPair in modSubscribers)
            {
                string mod = modPair.Key;
                LogModEventSubscribers(mod, modPair.Value);
            }

            OMC.Log("==== End of ALL MODS ====");
        }

        /// <summary>
        /// Logs a specific mod's events, subscribers, and variable types.
        /// </summary>
        public static void LogModSubscribers(string modNamespace)
        {
            var modSubscribers = EventBus.GetModSubscribers();

            if (!modSubscribers.TryGetValue(modNamespace, out var modEvents))
            {
                OMC.Log($"No subscribers found for mod '{modNamespace}'");
                return;
            }

            LogModEventSubscribers(modNamespace, modEvents);
        }

        /// <summary>
        /// Internal helper to log a mod's event dictionary.
        /// </summary>
        private static void LogModEventSubscribers(string modNamespace, Dictionary<string, List<Action<EventPayload?>>> modEvents)
        {
            OMC.Log($"==== EventBus Data: Mod '{modNamespace}' ====");
            foreach (var evtPair in modEvents)
            {
                string eventName = evtPair.Key;
                var handlers = evtPair.Value;

                OMC.Log($"Event: '{eventName}' | Subscriber Count: {handlers.Count}");

                int idx = 0;
                foreach (var handler in handlers)
                {
                    string methodName = handler.Method.Name;
                    string targetType = handler.Target?.GetType().Name ?? "static";
                    OMC.Log($"  Subscriber {++idx}: TargetType={targetType}, Method={methodName}");
                }
            }
            OMC.Log($"==== End of Mod '{modNamespace}' ====");
        }

        /// <summary>Logs all subscriber namespaces and their events.</summary>
        public static void LogAllSubscribers()
        {
            var modSubscribers = EventBus.GetModSubscribers();

            OMC.Log("==== EventBus: All Subscribers ====");
            foreach (var modPair in modSubscribers)
            {
                string mod = modPair.Key;
                foreach (var evtPair in modPair.Value)
                {
                    string evt = evtPair.Key;
                    int count = evtPair.Value.Count;
                    OMC.Log($"Mod: {mod} | Event: {evt} | Subscribers: {count}");
                }
            }
            OMC.Log("==== End of Subscribers ====");
        }

        /// <summary>Logs all subscribers for a specific event in a specific mod namespace.</summary>
        public static void LogEventSubscribers(string modNamespace, string eventName)
        {
            var modSubscribers = EventBus.GetModSubscribers();

            if (!modSubscribers.TryGetValue(modNamespace, out var modEvents))
            {
                OMC.Log($"Mod '{modNamespace}' not found.");
                return;
            }

            if (!modEvents.TryGetValue(eventName, out var handlers))
            {
                OMC.Log($"Event '{eventName}' not found in mod '{modNamespace}'.");
                return;
            }

            OMC.Log($"==== EventBus: Subscribers for '{modNamespace}.{eventName}' ====");
            int idx = 0;
            foreach (var handler in handlers)
            {
                string methodName = handler.Method.Name;
                string targetType = handler.Target?.GetType().Name ?? "static";
                OMC.Log($"Subscriber {++idx}: TargetType={targetType}, Method={methodName}");
            }
            OMC.Log("==== End of Event Subscribers ====");
        }

        /// <summary>Logs the contents of a payload with key names and value types.</summary>
        public static void LogPayload(EventPayload? payload)
        {
            if (payload == null)
            {
                OMC.Log("[EventBusDataPresenter@LogPayload] Payload is null.");
                return;
            }

            OMC.Log("---- Payload Contents ----");
            foreach (var kv in payload)
            {
                string key = kv.Key;
                string typeName = kv.Value?.GetType().Name ?? "null";
                string valueStr = kv.Value?.ToString() ?? "null";
                OMC.Log($"Key='{key}' | Type={typeName} | Value={valueStr}");
            }
            OMC.Log("--------------------------");
        }
    }
}
