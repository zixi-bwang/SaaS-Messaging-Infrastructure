using System;
using System.Collections.Generic;
using System.Text;

namespace Decoupling.EventBrokers
{
    public interface IEventHandler<TRequest> 
        where TRequest : IEventDataModel
    {
        void Process(TRequest data);
    }

    public interface IEventHandler<TRequest, TResponse>
        where TRequest : IEventDataModel
    {
        TResponse Process(TRequest data);
    }
}
