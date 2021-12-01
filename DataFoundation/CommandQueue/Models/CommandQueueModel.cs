using System;

namespace ContentFoundation.EventWebHook.Models
{
    public class CommandQueueModel
    {
        public Guid Id { get; set; }
        public Guid AppId { get; set; }
        public Guid StatusTermId { get; set; }
        public Guid StatusMessage { get; set; }
        public string Label { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Processor { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
