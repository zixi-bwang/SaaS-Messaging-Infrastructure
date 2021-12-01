using Repository.Enums;
using System;

namespace ContentFoundation.CommandQueue.Managers
{
    public partial class CommandQueueManager
    {
        public void Acknowledge(Guid id)
        {
            var msg = _dc.CommandQueue.Find(id);
            if (msg != null)
            {
                msg.StatusTermId = BuiltInCommandQueueStatus.Processing;
                msg.UpdatedTime = DateTime.UtcNow;
            }
            _dc.SaveChanges();
        }
    }
}
