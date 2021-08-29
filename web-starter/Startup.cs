using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using web_starter.Services;
using web_starter.Formatter;
using web_starter.Extensions;
using web_starter.Extensions.ErrorHandling;

namespace infrastructure
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddControllers(x =>
            {
                x.InputFormatters.Insert(x.InputFormatters.Count, new TextPlainInputFormatter());
                x.AllowEmptyInputInBodyModelBinding = true;
                x.Filters.Add(typeof(ErrorHandlingFilter));
            });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.RespectBrowserAcceptHeader = true;
            })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                })
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddControllersAsServices();

            services.Configure<ApiBehaviorOptions>(options =>
            {
               options.InvalidModelStateResponseFactory = ctx =>
               {
                   string message = "Unknown Error";

                   foreach (var state in ctx.ModelState)
                   {
                       if (state.Value.ValidationState == ModelValidationState.Invalid)
                       {
                           message = state.Value.Errors[0].ErrorMessage;
                           break;
                       }
                   }
                   throw new ValidationException(message);
               };
            });

            //services.addswa

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase(new PathString("/saas-domain"));

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
#if DEBUG
            app.UseCors("myCors");
#endif

            app.UseMiddleware<HttpRequestMiddleware>();

            app.UseHealthChecks($"/health");

            app.UseMvc();

            
       }
    }
}
