using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using PocOrleans.GrainInterface;

namespace PocOrleans
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IClusterClient>(CreateClusterClient);
        }

        private IClusterClient CreateClusterClient(IServiceProvider serviceProvider)
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering(serviceId: Declearation.OrleansSiloServiceId)
                .ConfigureApplicationParts(parts =>
                    {
                        parts.AddApplicationPart(typeof(IValueGrain).Assembly).WithReferences();
                        parts.AddApplicationPart(typeof(IPlayerGrain).Assembly).WithReferences();;
                    })
                .ConfigureLogging(_ => _.AddConsole())
                .Build();

            client.Connect().Wait();
            return client;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                
                app.UseDeveloperExceptionPage();
            }

            app
                .UseMvc()
                .UseMvcWithDefaultRoute();

            app.Run(async (context) => { await context.Response.WriteAsync("Hello World!"); });
        }
    }
}