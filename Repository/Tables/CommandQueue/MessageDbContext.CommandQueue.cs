using Microsoft.EntityFrameworkCore;
using Repository.Tables.EventQueue;
//using Dapper;
//using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Repository
{
    public partial class MessageDbContext : DbContext
    {
        public virtual DbSet<CommandQueueRecord> CommandQueue { get; set; }
    }
}




