using Decoupling.EventBrokers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Decoupling.Module
{
    public abstract class ModuleBase
    {
        public bool Initialized { get; set; }
        public IServiceProvider ServiceProvider => Decouplers.Decoupler.ServiceProvider;

        protected T GetService<T>(IServiceProvider provider = null)
        {
            if (provider != null)
                return provider.GetService<T>();
            else
                return ServiceProvider.GetService<T>();
        }

        public virtual void RegisterCronJob(IApplicationBuilder app)
        {
        }

        protected EnvType CurrentEnv
        {
            get
            {
                return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") switch
                {
                    "Local" => EnvType.Local,
                    "UnitTest" => EnvType.UnitTest,
                    "Testing" => EnvType.Testing,
                    "Development" => EnvType.Development,
                    "Staging" => EnvType.Staging,
                    "PreRelease" => EnvType.PreRelease,
                    "TestingPool" => EnvType.TestingPool,
                    "Prod" => EnvType.Prod,
                    "Live" => EnvType.Live,
                    _ => EnvType.Debug
                };
            }
        }
    }
}
