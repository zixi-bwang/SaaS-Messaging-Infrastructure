using System;

namespace ContentFoundation.CommandQueue.Models
{
    public class CommandQueueEnqueueModel
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Processor { get; set; }
        public string Label { get; set; }
        public Guid AppId { get; set; }
    }
}
