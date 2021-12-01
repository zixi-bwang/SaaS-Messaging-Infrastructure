using ContentFoundation.CommandQueue.Models;
using System.Linq;
using Repository.Tables.EventQueue;
using System;
using Repository.Enums;

namespace ContentFoundation.CommandQueue.Managers
{
    public partial class CommandQueueManager
    {
        public Guid Enqueue(CommandQueueEnqueueModel model)
        {
            // check if exists
            /*var content = JsonConvert.SerializeObject(model.Content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });*/
            var hash = Utilities.Util.MD5(model.Content);
            /*var command = _dc.CommandQueue.FirstOrDefault(x => x.OrderId == model.OrderId 
                && x.ContentHash == hash
                && x.StatusTermId == BuiltInCommandQueueStatus.Enqueued);
            if (command != null)
                return command.Id;*/

            var record = new CommandQueueRecord
            {
                Content = model.Content,
                ContentHash = hash,
                Label = model.Label,
                Processor = model.Processor,
                StatusTermId = BuiltInCommandQueueStatus.Enqueued,
                Subject = model.Subject,
                OrderId = model.OrderId,
                WorkOrderId = model.WorkOrderId
            };

            if (_currentUser.UserId != Guid.Empty)
            {
                record.OriginatedUserId = _currentUser.UserId;
            }

            _dc.CommandQueue.Add(record);
            _dc.SaveChanges();

            return record.Id;
        }
    }
}
