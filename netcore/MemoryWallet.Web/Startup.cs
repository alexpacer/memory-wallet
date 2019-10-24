using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.Routing;
using MemoryWallet.Lib;
using MemoryWallet.ProcessManager.Actor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MemoryWallet.Web
{
    public class Startup
    {
        public delegate ActorSelection PlayerManagerProvider();
        public delegate IActorRef PlayerBookProxyProvider();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection s)
        {
            s.AddMvc(c => { c.EnableEndpointRouting = false; });
            s.AddSingleton<ActorSystem>(s =>
            {
                var fs = s.GetService<IBaseFileFactory>();

                var conf = ConfigurationFactory.ParseString(fs.ReadRelative("web.hocon"));
                
                var system = ActorSystem.Create("sbk", conf);

                var playerManagers = system.ActorOf(
                    Props.Empty.WithRouter(FromConfig.Instance), 
                    "player-managers");
                
                var playerbook = system.ActorOf(ClusterSingletonProxy.Props(
                        singletonManagerPath: "/user/playerbook",
                        settings: ClusterSingletonProxySettings.Create(system).WithRole("player-manager")),
                    name: "playerbook-proxy");

                var logger = system.Log;
                logger.Info($"Player Manager:  ------->>>  {playerManagers}");
                
                logger.Info($"Playerbook ----->> {playerbook}");

                logger.Info($"{system.Name}");
                return system;
            });

            s.AddTransient<PlayerManagerProvider>(s =>
            {
                var sys = s.GetService<ActorSystem>();
                return () => sys.ActorSelection("/user/player-managers");
            });
//
//            s.AddTransient<PlayerBookProxyProvider>(s =>
//            {
//                var sys = s.GetService<ActorSystem>();
//                var playerbook = sys.ActorOf(ClusterSingletonProxy.Props(
//                        singletonManagerPath: "/user/playerbook",
//                        settings: ClusterSingletonProxySettings.Create(sys).WithRole("player-manager")),
//                    name: "playerbook-proxy-web");
//                return () => playerbook;
//            });
            
            s.BuildCommonDependency();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            lifetime.ApplicationStarted.Register(() =>
            {
                // Initialize logger
                var logger = app.ApplicationServices.GetService<ILogger>();

                // Initialize System Actor
                var system = app.ApplicationServices.GetService<ActorSystem>();

                // mount router in system
                                //var hub = app.ApplicationServices.GetService<IHubContext<SoftpaqHub>>();

//                system.ActorOf(MotherShip.Props(), ActorPaths.Mothership.Name);
//                system.ActorOf(HubBroadCaster.Props(hub), ActorPaths.SignalrHub.Name);
//                system.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), ActorPaths.Hub.Name);

//                var dlActor = system.ActorOf(DeadLetterMonitorActor.Props(), "deadletter-monitor");

//                system.EventStream.Subscribe(dlActor, typeof(DeadLetter));
            });

            // Akka System Shutdown
            lifetime.ApplicationStopping.Register(() =>
            {
                var logger = app.ApplicationServices.GetService<ILogger>();
                var system = app.ApplicationServices.GetService<ActorSystem>();
                logger.Warning("SmrWeb is being terminated... leaving cluster");

                var cluster = Akka.Cluster.Cluster.Get(system);
                cluster.Leave(cluster.SelfAddress);

                CoordinatedShutdown.Get(system)
                    .Run(CoordinatedShutdown.ClrExitReason.Instance)
                    .Wait();

                logger.Warning("SmrWeb Terminated");
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            
            app.UseMvcWithDefaultRoute();
            
            app.UseAuthorization();
        }
    }
}