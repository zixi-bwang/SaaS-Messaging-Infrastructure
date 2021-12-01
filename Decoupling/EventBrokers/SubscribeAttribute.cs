using System;
using System.Collections.Generic;
using System.Text;

namespace Decoupling.EventBrokers
{
    public class SubscribeAttribute : Attribute
    {
        /// <summary>
        /// The smaller the value, the higher the priority
        /// </summary>
        public int Priority { get; set; } = 1;
    }
}
