using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository.Tables.EventQueue
{
    [Table("CommandQueue")]
    public class CommandQueueRecord : TimedBaseRecord
    {
        [Required]
        public DateTime CreatedTime { get; set; }

        [Required]
        [StringLength(64)]
        public string Subject { get; set; }

        [Required]
        [StringLength(2048)]
        public string Content { get; set; }

        [Required]
        [StringLength(32)]
        public string ContentHash { get; set; }
        
        [Required]
        public string Processor { get; set; }

        [Required]
        public Guid StatusTermId { get; set; }
        [StringLength(256)]
        public string StatusMessage { get; set; }
        [Required]
        public Guid VendorId { get; set; }
        [StringLength(32)]
        public string Label { get; set; }
        public byte NumOfExecution { get; set; }
        public Guid? WorkOrderId { get; set; }
        public Guid? OriginatedUserId { get; set; }
        public CommandQueueRecord() : base()
        {
            CreatedTime = DateTime.UtcNow;
        }
    }
}
