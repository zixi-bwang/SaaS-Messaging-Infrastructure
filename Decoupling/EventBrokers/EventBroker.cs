using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
namespace Decoupling.EventBrokers
{
    public class EventBroker
    {
        IServiceProvider provider;
        
        public EventBroker(IServiceProvider x)
        {
            provider = x;
        }

        public TResponse Send<TRequest, TResponse>(TRequest data) 
            where TRequest : IEventDataModel
        {
            // Notify only one receiver
            var handlers = provider.GetServices<IEventHandler<TRequest, TResponse>>().ToList();
            if (handlers.Count > 1)
            {
                Console.WriteLine($"There are multiple receivers for {typeof(TRequest).Name}.");
                return handlers.First().Process(data);
            }
            else if(handlers.Count == 0)
            {
                Console.WriteLine($"Can't find any receiver for {typeof(TRequest).Name}.");
                return default;
            }
            else
            {
                return handlers.First().Process(data);
            }
        }

        public void Publish<T>(T data) where T : IEventDataModel
        {
            // Notify all subscribers
            var handlers = provider.GetServices<IEventHandler<T>>().ToList();
            if(handlers.Count > 0)
            {
                var queue = new Queue<IEventHandler<T>>(handlers);
                Process(queue, handlers, data);
            }
            else
            {
                Console.WriteLine($"Can't find any handlers for {typeof(T).Name}.");
            }
        }

        void Process<T>(Queue<IEventHandler<T>> queue, List<IEventHandler<T>> handlers, T data) where T : IEventDataModel
        {
            while (queue.Any())
            {
                var handler = queue.Dequeue();
                var priority = handler.GetType().GetCustomAttribute<EventHandlerPriorityAttribute>();
                if (!string.IsNullOrEmpty(priority?.After))
                {
                    queue.Enqueue(handlers.First(x => x.GetType().Name == priority.After));
                    Process(queue, handlers, data);
                }

                if(priority != null && priority.Disabled)
                {
                    handlers.Remove(handler);
                    continue;
                }

                if (handlers.Contains(handler))
                {
                    handler.Process(data);
                    Console.WriteLine($"Triggered handler {handler.GetType().Name}.");
                    handlers.Remove(handler);
                }
            }
        }
    }
}
