using Microsoft.EntityFrameworkCore;
using Repository.Tables.EventQueue;

namespace Repository
{
    public partial class VendorDbContext
    {
        public DbSet<CommandQueueRecord> CommandQueue => base.Table<CommandQueueRecord>();
    }
}




