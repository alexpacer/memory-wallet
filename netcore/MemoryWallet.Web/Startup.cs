using System;
using Akka.Actor;
using MemoryWallet.Lib;
using MemoryWallet.Web.Actors;
using MemoryWallet.Web.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MemoryWallet.Web
{
    public class Startup
    {
        public delegate ActorSelection PlayerManagerProvider();

        public delegate IActorRef PlayerBookProxyProvider();

        public delegate ActorSelection WebHubProvider();

        public static IServiceProvider Provider { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection s)
        {
            s.AddMvc(c => { c.EnableEndpointRouting = false; });
            s.AddSignalR();
            s.BuildCommonDependency();
            // System Actor depends on common lib services, hence ordering is important here
            s.AddSystemActor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            Provider = app.ApplicationServices;
            
            lifetime.ApplicationStarted.Register(() => SystemActor.ApplicationStarted(app));

            lifetime.ApplicationStopping.Register(() => SystemActor.ApplicationStopping(app));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseRouting();
            app.UseEndpoints(ep =>
            {
                ep.MapHub<PlayerHub>("/playerhub");
                ep.MapControllers();
            });

            app.UseStaticFiles();
            app.UseAuthorization();
        }
    }
}