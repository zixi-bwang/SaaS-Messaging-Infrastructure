using Decoupling.Decouplers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class DecouplerServiceExtensions
    {
        public static void AddDecoupler(this IServiceCollection services, IConfiguration config, Func<Type, bool> registerIoC)
        {
            var decouper = new Decoupler(services, config, registerIoC);
            services.AddSingleton(provider =>
            {
                decouper.SetServiceProvider(provider.CreateScope().ServiceProvider);
                return decouper;
            });
        }

        public static void UseDecoupler(this IApplicationBuilder app)
        {
            var decoupler = app.ApplicationServices.GetService<Decoupler>();
            decoupler.InitModules(app);
        }
    }
}
