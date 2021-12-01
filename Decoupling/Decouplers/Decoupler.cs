using AutoMapper.Internal;
using Decoupling.EventBrokers;
using Decoupling.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Console = Colorful.Console;

namespace Decoupling.Decouplers
{
    public partial class Decoupler
    {
        IServiceProvider serviceProvider;
        IServiceCollection services;
        IConfiguration config;
        DecouplerSettings settings;
        List<IModule> modules;
        [ThreadStatic]
        public static IServiceProvider ServiceProvider;

        public Decoupler(IServiceCollection services, IConfiguration config, Func<Type, bool> registerIoC)
        {
            Console.WriteLine("Initializing decoupler ...");

            this.services = services;
            this.config = config;

            var settings = new DecouplerSettings();
            config.Bind("Decoupler", settings);
            services.AddSingleton(settings);

            // register LiteEventManager
            services.AddScoped((IServiceProvider x) =>
            {
                return new EventBroker(x);
            });

            this.settings = settings;
            modules = new List<IModule>();

            var executingDir = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;

            settings.Assemblies.ToList().ForEach(assemblyName =>
            {
                var assemblyPath = Path.Combine(executingDir, assemblyName + ".dll");
                if (File.Exists(assemblyPath))
                {
                    var assembly = Assembly.Load(assemblyName);
                    services.AddAutoMapper(assembly);

                    // auto scan and inject IoC
                    assembly.GetTypes().ForAll(x =>
                    {
                        if (x.IsPublic && registerIoC(x))
                            services.AddScoped(x);
                    });

                    var modules = assembly.GetTypes()
                        .Where(x => x.GetInterface(nameof(IModule)) != null)
                        .Select(x => Activator.CreateInstance(x) as IModule)
                        .ToList();

                    foreach (var module in modules)
                    {
                        module.RegisterDI(services, config);
                    }

                    this.modules.AddRange(modules);
                }
                else
                {
                    Console.WriteLine($"Can't find assemble {assemblyPath}.");
                }
            });
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider GetServiceProvider()
        {
            return serviceProvider;
        }

        public T GetService<T>()
            => serviceProvider.GetService<T>();

        public void InitModules(IApplicationBuilder app)
        {
            int loadedModuleCount = 0;
            SetSystemUser(settings.Identity);
            foreach (var module in modules)
            {
                InitModule(app, module);
                loadedModuleCount++;
            }

            Console.WriteLine($"Loaded {loadedModuleCount} modules in Decoupler.", Color.Green);
        }

        /// <summary>
        /// Init module recursively according to dependency attribute.
        /// </summary>
        /// <param name="module"></param>
        void InitModule(IApplicationBuilder app, IModule module)
        {
            if (module.Initialized)
                return;

            // Solve module dependency
            var attr = module.GetType().GetCustomAttribute<ModuleDependencyAttribute>();
            if (attr != null)
            {
                foreach (var moduleName in attr.ModuleNames)
                {
                    var dependModule = modules.FirstOrDefault(x => x.GetType().Name == moduleName);
                    if (dependModule == null)
                        throw new Exception($"Can't solve dependency for {module.GetType().Name} due to {moduleName} doesn't exist.");

                    InitModule(app, dependModule);
                }
            }

            // Setup database
            if(!settings.SkipSetupDb)
                module.SetupDb();

            // Register cronjob
            module.RegisterCronJob(app);

            module.Initialized = true;
            Console.WriteLine($"Initialized module: {module.GetType().Name}.", Color.Green);
        }
    }
}
