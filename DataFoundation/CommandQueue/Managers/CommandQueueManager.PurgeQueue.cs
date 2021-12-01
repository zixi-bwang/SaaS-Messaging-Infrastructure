using EntityFrameworkCore.BootKit;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ContentFoundation.CommandQueue.Managers
{
    public partial class CommandQueueManager
    {
        public void PurgeQueue()
        {
            var msg = _dc.CommandQueue
                .Where(x => x.UpdatedTime < DateTime.UtcNow.AddDays(-14))
                .ToList();
            _dc.CommandQueue.RemoveRange(msg);
            _dc.SaveChanges();
        }
    }
}
