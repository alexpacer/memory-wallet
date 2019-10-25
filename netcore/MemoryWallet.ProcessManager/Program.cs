using System;
using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Routing;
using MemoryWallet.ProcessManager.Actor;
using Microsoft.Extensions.DependencyInjection;

namespace MemoryWallet.ProcessManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello from MemoryWallet!");

            var system = DI.Provider.GetService<ActorSystem>();

            var logger = system.Log;
            
            logger.Info($"Joining cluster {system.Name}");
            
            // Singleton actor that keeps book of record of a player
            system.ActorOf(ClusterSingletonManager.Props(
                singletonProps: PlayerBook.Props(),
                terminationMessage: PoisonPill.Instance,
                settings: ClusterSingletonManagerSettings.Create(system).WithRole("player-manager")
            ), "playerbook");
            
            
            system.ActorOf(ClusterSingletonProxy.Props(
                    singletonManagerPath: "/user/playerbook",
                    settings: ClusterSingletonProxySettings.Create(system)
                        .WithRole("player-manager")),
                name: "playerbook-proxy");
            
            // create local player manager instances with router.
            var pm = system.ActorOf(PlayerManagerActor.Props(), "player-manager");
//            var pm = system.ActorOf(
//                PlayerManagerActor.Props().WithRouter(FromConfig.Instance),
//                "player-manager");
//
//            
            logger.Info($"{pm.Path} created.");
            
            Console.ReadLine();

            CoordinatedShutdown.Get(system)
                .Run(CoordinatedShutdown.ClrExitReason.Instance)
                .Wait();
            
        }
    }
}