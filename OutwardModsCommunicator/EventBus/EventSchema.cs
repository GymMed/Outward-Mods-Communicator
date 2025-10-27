using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardModsCommunicator.EventBus
{
    public class EventSchema
    {
        // fieldName -> Type
        public Dictionary<string, Type> Fields { get; } = new Dictionary<string, Type>();

        // Field name -> Description (optional)
        public Dictionary<string, string> Descriptions { get; } = new Dictionary<string, string>();

        public EventSchema() { }

        // Build schema from an EventPayload where values are Types (or from runtime payload types)
        public EventSchema(EventPayload payload, bool treatValuesAsTypes = false)
        {
            if (payload == null) return;
            foreach (var kv in payload)
            {
                if (treatValuesAsTypes && kv.Value is Type t)
                    Fields[kv.Key] = t;
                else if (kv.Value is Type tt)
                    Fields[kv.Key] = tt;
                else if (kv.Value != null)
                    Fields[kv.Key] = kv.Value.GetType();
                else
                    Fields[kv.Key] = typeof(object);
            }
        }

        public void AddField(string name, Type type, string? description = null)
        {
            Fields[name] = type;
            if (!string.IsNullOrWhiteSpace(description))
                Descriptions[name] = description!;
        }

        public string? GetDescription(string field)
        {
            return Descriptions.TryGetValue(field, out var desc) ? desc : null;
        }
    }
}
