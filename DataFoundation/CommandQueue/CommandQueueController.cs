using System;
using System.Collections.Generic;
using System.Linq;
using ContentFoundation.CommandQueue.Managers;
using ContentFoundation.CommandQueue.Models;
using ContentFoundation.DbSetup;
using ContentFoundation.EventWebHook.Models;
using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Repository.Enums;
using Repository.Tables;
using SharedModels.Common;

namespace ContentFoundation.EventWebHook
{
    /// <summary>
    /// Event web hook
    /// </summary>
    public class CommandQueueController : RootController
    {
        private readonly CommandQueueManager commandQueueMgr;
        private readonly OrderDbContext _dc;

        public CommandQueueController(CommandQueueManager commandQueueMgr, OrderDbContext dc)
        {
            this.commandQueueMgr = commandQueueMgr;
            _dc = dc;
        }

        /// <summary>
        /// Push a event queue manually
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/command/enqueue")]
        [DbTransactionScopeAdvice]
        public Guid EnqueueCommand([FromBody] CommandQueueEnqueueModel model)
        {
            return commandQueueMgr.Enqueue(model);
        }

        /// <summary>
        /// Pull message from event queue.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/commands/dequeue/{count}")]
        [DbTransactionScopeAdvice]
        public List<CommandQueueModel> DequeueCommands([FromRoute] int count)
        {
            return commandQueueMgr.Dequeue(count, BuiltInCommandQueueStatus.Enqueued);
        }
         
        [HttpPost("/command/{id}/proceed")]
        [DbTransactionScopeAdvice]
        public void ProceedCommand([FromRoute] Guid id)
        {
            commandQueueMgr.Proceed(id);
        }

        [HttpPost("/commands/dequeue/{count}/proceed")]
        public void DequeueAndProceedCommands([FromRoute] int count)
        {
            var commands = commandQueueMgr.Dequeue(count, BuiltInCommandQueueStatus.Enqueued);
            foreach(var command in commands)
            {
                _dc.Transaction<IOrderTable>(() => commandQueueMgr.Proceed(command.Id));
            }
        }

        [HttpPost("/commands/dequeue/{count}/retry")]
        public void DequeueAndRetryCommands([FromRoute] int count)
        {
            var commands = commandQueueMgr.Dequeue(count, BuiltInCommandQueueStatus.Failed);
            foreach (var command in commands)  
            {
                _dc.Transaction<IOrderTable>(() => commandQueueMgr.Proceed(command.Id));
            }
        }

        /// <summary>
        /// Acknowledge command completed result.
        /// </summary>
        /// <param name="id"></param>
        [HttpPatch("/command/{id}/acknowledge")]
        [DbTransactionScopeAdvice]
        public void Acknowledge([FromRoute] Guid id)
        {
            commandQueueMgr.Acknowledge(id);
        }

        /// <summary>
        /// Clean processed message priority to 14 days.
        /// </summary>
        [HttpDelete("/commands")]
        [DbTransactionScopeAdvice]
        public void PurgeQueue()
        {
            commandQueueMgr.PurgeQueue();
        }
    }
}
