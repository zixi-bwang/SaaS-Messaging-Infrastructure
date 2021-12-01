using ContentFoundation.DbSetup;
using ContentFoundation.EventWebHook.Models;
using EntityFrameworkCore.BootKit;
using Repository.Enums;
using Repository.Tables.EventQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using static Utilities.Util;

namespace ContentFoundation.CommandQueue.Managers
{
    public partial class CommandQueueManager
    {
        public List<CommandQueueModel> Dequeue(int count, Guid statusTermId)
        {
            // lock table
            /*SELECT TOP 1 Id
                FROM CommandQueue
                WITH(UPDLOCK)
                WHERE ProcessedTime IS NULL
                ORDER BY UpdatedTime*/
            var sql = @$"SELECT TOP {count} Id 
                FROM {GetTableNameByAttribute<CommandQueueRecord>()}
                WITH(UPDLOCK)
                WHERE StatusTermId='{statusTermId}' AND NumOfExecution<{MAX_RETRY_TIMES + 1}";
            var result = _dc.ExecuteSqlCommand<CommandQueueRecord>(sql);

            var records = _dc.CommandQueue.Where(x => x.StatusTermId == statusTermId && x.NumOfExecution < MAX_RETRY_TIMES)
                .OrderBy(x => x.CreatedTime)
                .Take(count)
                .ToList();

            foreach(var record in records)
            {
                record.UpdatedTime = DateTime.UtcNow;
                record.StatusTermId = BuiltInCommandQueueStatus.Dequeued;
            }
            _dc.SaveChanges();

            return records.Select(x => new CommandQueueModel
            {
                Id = x.Id,
                VendorId = x.VendorId,
                StatusTermId = x.StatusTermId,
                Subject = x.Subject,
                Label = x.Label,
                Processor = x.Processor,
                UpdatedTime = x.UpdatedTime,
            }).ToList();
        }
    }
}
