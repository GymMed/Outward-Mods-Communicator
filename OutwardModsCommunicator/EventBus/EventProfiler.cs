using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OutwardModsCommunicator.EventBus
{
    public static class EventProfiler
    {
        public static bool Enabled { get; private set; }
        public static bool InstantLogging { get; private set; }

        private static readonly Dictionary<string, EventProfileData> _profiles = new();

        // ============================================================
        // =================== INITIALIZATION =========================
        // ============================================================

        public static void Initialize()
        {
            // Subscribe to config changes
            OMC.EnableEventsProfiler.SettingChanged += (_, _) =>
            {
                Enabled = OMC.EnableEventsProfiler.Value;
                OMC.Log($"[EventProfiler] {(Enabled ? "Enabled" : "Disabled")} via config.");
            };

            // Optional instant logging config
            if (OMC.InstantLogEventsProfileData != null)
            {
                OMC.InstantLogEventsProfileData.SettingChanged += (_, _) =>
                {
                    InstantLogging = OMC.InstantLogEventsProfileData.Value;
                    OMC.Log($"[EventProfiler] Instant logging {(InstantLogging ? "enabled" : "disabled")} via config.");
                };
                InstantLogging = OMC.InstantLogEventsProfileData.Value;
            }

            Enabled = OMC.EnableEventsProfiler.Value;
        }

        // ============================================================
        // =================== RECORDING LOGIC ========================
        // ============================================================

        /// <summary>
        /// Records elapsed time for an event.
        /// </summary>
        public static void Record(string modNamespace, string eventName, double elapsedMs)
        {
            if (!Enabled) return;

            string key = $"{modNamespace}.{eventName}";
            if (!_profiles.TryGetValue(key, out var data))
                _profiles[key] = data = new EventProfileData(modNamespace, eventName);

            data.Record(elapsedMs);

            if (InstantLogging)
                OMC.Log($"[EventProfiler] {key} subscribers executed in {elapsedMs:F2} ms");
        }

        /// <summary>
        /// Records a subscriber’s individual execution time.
        /// </summary>
        public static void RecordSubscriber(string modNamespace, string eventName, string subscriberId, double elapsedMs)
        {
            if (!Enabled) return;

            string key = $"{modNamespace}.{eventName}";
            if (!_profiles.TryGetValue(key, out var data))
                _profiles[key] = data = new EventProfileData(modNamespace, eventName);

            data.RecordSubscriber(subscriberId, elapsedMs);

            if (InstantLogging)
                OMC.Log($"[EventProfiler] {key} subscriber '{subscriberId}' took {elapsedMs:F2} ms");
        }

        // ============================================================
        // =================== LOGGING METHODS ========================
        // ============================================================

        /// <summary>
        /// Logs all profiling data.
        /// </summary>
        public static void LogProfiles()
        {
            if (!Enabled)
            {
                OMC.Log("[EventProfiler] Profiler is disabled.");
                return;
            }

            OMC.Log("==== EventBus Profiler Summary ====");
            foreach (var kv in _profiles.Values.OrderByDescending(p => p.TotalMs))
            {
                OMC.Log($"Event '{kv.ModNamespace}.{kv.EventName}' — Calls: {kv.CallCount}, Total: {kv.TotalMs:F2} ms, Max: {kv.MaxMs:F2} ms, Avg: {(kv.TotalMs / kv.CallCount):F2} ms");

                if (kv.SubscriberTimes.Count > 0)
                {
                    foreach (var s in kv.SubscriberTimes)
                    {
                        OMC.Log($"    Subscriber {s.Key} — Calls: {s.Value.CallCount}, Total: {s.Value.TotalMs:F2} ms, Max: {s.Value.MaxMs:F2} ms, Avg: {(s.Value.TotalMs / s.Value.CallCount):F2} ms");
                    }
                }
            }
            OMC.Log("==== End of Profiler Summary ====");
        }

        // ============================================================
        // =================== DATA CLASSES ===========================
        // ============================================================

        public class EventProfileData
        {
            public string ModNamespace { get; }
            public string EventName { get; }

            public int CallCount { get; private set; }
            public double TotalMs { get; private set; }
            public double MaxMs { get; private set; }

            public readonly Dictionary<string, SubscriberProfile> SubscriberTimes = new();

            public EventProfileData(string modNamespace, string eventName)
            {
                ModNamespace = modNamespace;
                EventName = eventName;
            }

            public void Record(double elapsedMs)
            {
                CallCount++;
                TotalMs += elapsedMs;
                if (elapsedMs > MaxMs) MaxMs = elapsedMs;
            }

            public void RecordSubscriber(string subscriberId, double elapsedMs)
            {
                if (!SubscriberTimes.TryGetValue(subscriberId, out var s))
                    SubscriberTimes[subscriberId] = s = new SubscriberProfile();

                s.Record(elapsedMs);
            }
        }

        public class SubscriberProfile
        {
            public int CallCount { get; private set; }
            public double TotalMs { get; private set; }
            public double MaxMs { get; private set; }

            public void Record(double elapsedMs)
            {
                CallCount++;
                TotalMs += elapsedMs;
                if (elapsedMs > MaxMs) MaxMs = elapsedMs;
            }
        }
    }
}
