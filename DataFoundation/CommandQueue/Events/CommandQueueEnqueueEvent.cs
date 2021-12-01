using ContentFoundation.CommandQueue.Models;
using Decoupling.EventBrokers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentFoundation.Events
{
    public class CommandQueueEnqueueEvent : IEventDataModel
    {
        public bool RunImmediately { get; set; }
        public CommandQueueEnqueueModel Data { get; set; }
    }
}
