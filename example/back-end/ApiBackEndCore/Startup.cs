using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultipartFormDataFormatterExtension;
using Newtonsoft.Json.Serialization;

namespace ApiBackEndCore
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
            services.AddOptions();


            services
                .AddMvc(options =>
                {
                    var a = options.FormatterMappings.GetMediaTypeMappingForFormat("multipart/form-data");
                    options.InputFormatters.Insert(0, new MultipartFormDataFormatter());
                })
                .AddJsonOptions(options =>
                {
                    var camelCasePropertyNamesContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ContractResolver = camelCasePropertyNamesContractResolver;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // Get logger instance.
            var logger = app.ApplicationServices.GetService<ILogger<Startup>>();
            
            // Enable mvc pipeline.
            app
                .UseMvcWithDefaultRoute();
        }
    }
}