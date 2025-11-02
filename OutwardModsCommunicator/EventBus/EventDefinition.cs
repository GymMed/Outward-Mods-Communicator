using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardModsCommunicator.EventBus
{
    public class EventDefinition
    {
        public EventSchema Schema { get; set; }
        public string Description { get; set; } = "";

        public EventDefinition(EventSchema? schema = null, string description = "")
        {
            Schema = schema ?? new EventSchema();
            Description = description;
        }
    }
}
