using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

namespace infrastructure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();

            Log.Information("Hello, world!");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                    //{
                    //    var config = configurationBuilder.Build();
                    //    configurationBuilder.AddConfiguration(
                    //        hostBuilderContext.HostingEnvironment,
                    //        config.GetValue());
                    //}).UseStartup<Startup>();

                    webBuilder.UseStartup<Startup>();
                });
    }
}
