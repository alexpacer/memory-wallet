using Akka.Actor;
using MemoryWallet.Lib;
using MemoryWallet.Web.Actors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MemoryWallet.Web.Data;
using MemoryWallet.Web.Hub;

namespace MemoryWallet.Web
{
    public class Startup
    {
        public delegate ActorSelection PlayerManagerProvider();

        public delegate IActorRef PlayerBookProxyProvider();

        public delegate ActorSelection WebHubProvider();
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.BuildCommonDependency();
            services.AddSignalR();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
            
            services.AddSystemActor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapHub<PlayerHub>("/playerhub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
