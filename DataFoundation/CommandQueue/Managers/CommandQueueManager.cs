using ContentFoundation.Events;
using ContentFoundation.User;
using Decoupling.EventBrokers;
using Repository;
using System;

namespace ContentFoundation.CommandQueue.Managers
{
    public partial class CommandQueueManager : IEventHandler<CommandQueueEnqueueEvent, Guid>
    {
        VendorDbContext _dc;
        ICurrentUser _currentUser;

        static byte MAX_RETRY_TIMES = 3;

        public CommandQueueManager(VendorDbContext dc,
            ICurrentUser user)
        {
            _dc = dc;
            _currentUser = user;
        }

        public Guid Process(CommandQueueEnqueueEvent data)
        {
            var cid = Enqueue(data.Data);

            // run command automatically for integration test
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing")
                data.RunImmediately = true;

#if DEBUG
            data.RunImmediately = true;
#endif

            if (data.RunImmediately)
                Proceed(cid);

            return cid;
        }
    }
}
