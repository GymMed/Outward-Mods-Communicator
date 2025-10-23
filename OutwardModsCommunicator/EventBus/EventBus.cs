using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardModsCommunicator.EventBus
{
    /// <summary>
    /// Provides a simple, namespaced, runtime event system for mods.
    /// </summary>
    public static class EventBus
    {
        // Dictionary<modNamespace, Dictionary<eventName, List<callbacks>>>
        private static readonly Dictionary<string, Dictionary<string, List<Action<EventPayload?>>>> _modSubscribers = new();

        // Provide a safe read-only copy for presenters
        public static IReadOnlyDictionary<string, Dictionary<string, List<Action<EventPayload?>>>> GetModSubscribers()
        {
            return _modSubscribers;
        }

        // Dictionary<modNamespace, Dictionary<eventName, EventPayload>>
        private static readonly Dictionary<string, Dictionary<string, EventPayload>> _publishedPayloads 
            = new Dictionary<string, Dictionary<string, EventPayload>>();

        // Provide a safe read-only copy for presenters
        public static IReadOnlyDictionary<string, Dictionary<string, EventPayload>> GetModPublishedPayloads()
        {
            return _publishedPayloads;
        }

        // Registered events metadata: Dictionary<modNamespace, Dictionary<eventName, EventSchema>>
        private static readonly Dictionary<string, Dictionary<string, EventSchema>> _registeredEvents
            = new Dictionary<string, Dictionary<string, EventSchema>>();


        // Accessor (read-only view) for presenters
        public static IReadOnlyDictionary<string, Dictionary<string, EventSchema>> GetRegisteredEvents()
        {
            return _registeredEvents;
        }

        /// <summary>
        /// Register with explicit schema object
        /// </summary>
        /// <param name="modNamespace"></param>
        /// <param name="eventName"></param>
        /// <param name="schema"></param>
        public static void RegisterEvent(string modNamespace, string eventName, EventSchema? schema = null)
        {
            if (!_registeredEvents.TryGetValue(modNamespace, out var modEvents))
                _registeredEvents[modNamespace] = modEvents = new Dictionary<string, EventSchema>();

            modEvents[eventName] = schema ?? new EventSchema();

            // ensure published slots exist (optional convenience)
            if (!_publishedPayloads.TryGetValue(modNamespace, out var modPublished))
                _publishedPayloads[modNamespace] = modPublished = new Dictionary<string, EventPayload>();
            if (!modPublished.ContainsKey(eventName))
                modPublished[eventName] = new EventPayload();

            OMC.Log($"[EventBus] Registered event '{modNamespace}.{eventName}'");
        }

        /// <summary>
        /// Register event convenience using array of (key, type)
        /// </summary>
        public static void RegisterEvent(string modNamespace, string eventName, params (string key, Type type)[] fields)
        {
            var schema = new EventSchema();
            foreach (var (k, t) in fields) schema.AddField(k, t);
            RegisterEvent(modNamespace, eventName, schema);
        }

        /// <summary>
        /// Another convenience: register from an EventPayload where values are Types (payload contains Type objects)
        /// </summary>
        public static void RegisterEventFromPayloadTypes(string modNamespace, string eventName, EventPayload payloadWithTypes)
        {
            var schema = new EventSchema(payloadWithTypes, treatValuesAsTypes: true);
            RegisterEvent(modNamespace, eventName, schema);
        }

        /// <summary>
        /// Subscribe to an event inside a specific mod's namespace.
        /// </summary>
        public static void Subscribe(string modNamespace, string eventName, Action<EventPayload?> callback)
        {
            if (!_modSubscribers.TryGetValue(modNamespace, out var modEvents))
                _modSubscribers[modNamespace] = modEvents = new Dictionary<string, List<Action<EventPayload?>>>();

            if (!modEvents.TryGetValue(eventName, out var handlers))
                modEvents[eventName] = handlers = new List<Action<EventPayload?>>();


            #if DEBUG
            OMC.Log($"Subscribed to event '{eventName}' from '{modNamespace}'");
            #endif
            handlers.Add(callback);
        }

        /// <summary>
        /// Unsubscribe a handler from a specific mod's event.
        /// </summary>
        public static void Unsubscribe(string modNamespace, string eventName, Action<EventPayload?> callback)
        {
            #if DEBUG
            OMC.Log($"Unsubscribed to event '{eventName}' from '{modNamespace}'");
            #endif
            if (_modSubscribers.TryGetValue(modNamespace, out var modEvents) &&
                modEvents.TryGetValue(eventName, out var handlers))
            {
                handlers.Remove(callback);
            }
        }

        /// <summary>
        /// Publish an event within a mod's namespace.
        /// </summary>
        public static void Publish(string modNamespace, string eventName, EventPayload? payload = null)
        {
            if (!_publishedPayloads.TryGetValue(modNamespace, out var modPublished))
                _publishedPayloads[modNamespace] = modPublished = new Dictionary<string, EventPayload>();

            modPublished[eventName] = payload != null ? new EventPayload(payload) : new EventPayload();

            if (!_modSubscribers.TryGetValue(modNamespace, out var modEvents))
                return;

            if (!modEvents.TryGetValue(eventName, out var handlers))
                return;

            Stopwatch? sw = null;
            if (OMC.EnableEventsProfiler.Value)
                sw = Stopwatch.StartNew();

            foreach (var handler in handlers)
            {
                Stopwatch? subSw = null;
                if (OMC.EnableEventsProfiler.Value)
                    subSw = Stopwatch.StartNew();

                try
                {
                    handler(payload);
                }
                catch (Exception ex)
                {
                    OMC.Log($"[EventBus] Error in '{modNamespace}.{eventName}' subscriber: {ex}", Enums.ENUM_LOG_LEVELS.Error);
                }

                if (OMC.EnableEventsProfiler.Value && subSw != null)
                {
                    subSw.Stop();
                    string subscriberInfo = $"{handler.Method.DeclaringType?.Name}.{handler.Method.Name}";
                    EventProfiler.RecordSubscriber(modNamespace, eventName, subscriberInfo, subSw.Elapsed.TotalMilliseconds);
                }
            }

            if (OMC.EnableEventsProfiler.Value && sw != null)
            {
                sw.Stop();
                EventProfiler.Record(modNamespace, eventName, sw.Elapsed.TotalMilliseconds);
            }
        }

        /// <summary>
        /// Unsubscribe all events for a given mod namespace.
        /// </summary>
        public static void ClearNamespace(string modNamespace)
        {
            _modSubscribers.Remove(modNamespace);
        }
    }
}
