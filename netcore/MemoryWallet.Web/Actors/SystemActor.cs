using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.Routing;
using MemoryWallet.Lib;
using MemoryWallet.Web.Hub;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MemoryWallet.Web.Actors
{
    
    public static class SystemActor
    {
        public static void AddSystemActor(this IServiceCollection services)
        {
            var sc = services.BuildServiceProvider();

            services.AddSingleton<ActorSystem>(s =>
            {
                var serilogger = s.GetService<ILogger>();
                var fs = sc.GetService<IBaseFileFactory>();
                var conf = ConfigurationFactory.ParseString(fs.ReadRelative("web.hocon"));
                var system = ActorSystem.Create("sbk", conf);

                // Create actors 
                var playerMgr = system.ActorOf(Props.Empty.WithRouter(FromConfig.Instance),
                    "player-managers");


                var playerHub = s.GetService<IHubContext<PlayerHub>>();

                // local hub actor
                var webHub = system.ActorOf(WebHub.Props(playerHub), "web-hub");
                // cluster-enabled hub router
                system.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "hub");

                var logger = system.Log;
                logger.Info($"Player Manager:  ------->>>  {playerMgr}");
                logger.Info($"{system.Name}");

                return system;
            });

            services.AddTransient<Startup.PlayerManagerProvider>(s =>
            {
                var sys = s.GetService<ActorSystem>();
                return () => sys.ActorSelection("/user/player-managers");
            });

            services.AddTransient<Startup.PlayerBookProxyProvider>(s =>
            {
                var sys = s.GetService<ActorSystem>();
                var logger = sys.Log;
                var playerBook = sys.ActorOf(ClusterSingletonProxy.Props(
                        singletonManagerPath: "/user/playerbook",
                        settings: ClusterSingletonProxySettings.Create(sys)
                            .WithRole("player-manager")),
                    name: "playerbook-proxy");
                logger.Info($"Playerbook ----->> {playerBook}");
                return () => playerBook;
            });

            services.AddTransient<Startup.WebHubProvider>(s =>
            {
                var sys = s.GetService<ActorSystem>();
                return () => sys.ActorSelection("/user/web-hub");
            });
        }
        
        public static void ApplicationStarted(IApplicationBuilder app)
        {
            // Initialize System Actor

            var hub = app.ApplicationServices.GetService<IHubContext<PlayerHub>>();
            
            var system = app.ApplicationServices.GetService<ActorSystem>();

            var log = system.Log;
            
            log.Info($"hub: {hub}");
        }

        /// <summary>
        /// Akka System Shutdown
        /// </summary>
        /// <param name="app"></param>
        public static void ApplicationStopping(IApplicationBuilder app)
        {
            var system = app.ApplicationServices.GetService<ActorSystem>();
            var logger = system.Log;
            logger.Warning("SbkWeb is being terminated... leaving cluster");

            var cluster = Akka.Cluster.Cluster.Get(system);
            cluster.Leave(cluster.SelfAddress);

            CoordinatedShutdown.Get(system)
                .Run(CoordinatedShutdown.ClrExitReason.Instance)
                .Wait();

            logger.Warning("SbkWeb Terminated");
        }
    }
}