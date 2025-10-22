using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardModsCommunicator.EventBus
{
    /// <summary>
    /// Generic data container for events.
    /// </summary>
    public class EventPayload : Dictionary<string, object>
    {
        public EventPayload() { }

        public EventPayload(EventPayload other)
        {
            if (other == null) return;
            foreach (var kv in other)
                this[kv.Key] = kv.Value;
        }

        public T Get<T>(string key, T? defaultValue = default)
        {
            if (TryGetValue(key, out var value) && value is T typed)
                return typed;
            return defaultValue!;
        }

        public void Set(string key, object value)
        {
            this[key] = value;
        }
    }
}
