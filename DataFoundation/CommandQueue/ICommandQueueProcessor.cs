using Repository.Tables.EventQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentFoundation.CommandQueue
{
    public interface ICommandQueueProcessor
    {
        Guid Proceed(CommandQueueRecord command);
    }
}
