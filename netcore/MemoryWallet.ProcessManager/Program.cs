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
            
            system.ActorOf(ClusterSingletonManager.Props(
                singletonProps: PlayerBook.Props(),
                terminationMessage: PoisonPill.Instance,
                settings: ClusterSingletonManagerSettings.Create(system).WithRole("player-manager")
            ), "playerbook");

            // create local player manager instances with router.
            var pm = system.ActorOf(
                PlayerManagerActor.Props().WithRouter(FromConfig.Instance),
                "player-manager");

            var clusterPm = system.ActorOf(
                Props.Empty.WithRouter(FromConfig.Instance), 
                "player-managers");
            
            
            logger.Info($"{pm.Path} created.");
            
            
            clusterPm.Tell(new PlayerManagerActor.HelloWorld("H---------------------->Dsadasdasdasdasdfewe3kojnf3erowgfnrewokqgnfrqewopnf"));
            
            

            Console.ReadLine();


            CoordinatedShutdown.Get(system)
                .Run(CoordinatedShutdown.ClrExitReason.Instance)
                .Wait();
            
        }
    }
}