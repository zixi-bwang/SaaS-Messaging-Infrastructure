using System;
using System.Collections.Generic;
using System.Text;

namespace Decoupling.EventBrokers
{
    public class EventHandlerPriorityAttribute : Attribute
    {
        public bool Disabled { get; set; } = false;
        public string After { get; set; }
    }
}
