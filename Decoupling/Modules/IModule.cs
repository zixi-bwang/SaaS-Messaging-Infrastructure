using Decoupling.EventBrokers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Decoupling.Module
{
    /// <summary>
    /// IModule is an interface to abstract the modulized function
    /// </summary>
    public interface IModule
    {
        bool Initialized { get; set; }

        /// <summary>
        /// Register dependency injection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        void RegisterDI(IServiceCollection services, IConfiguration config);

        /// <summary>
        /// Setup essential data for database
        /// </summary>
        void SetupDb();

        /// <summary>
        /// Register backgroud job
        /// </summary>
        void RegisterCronJob(IApplicationBuilder app);
    }
}
