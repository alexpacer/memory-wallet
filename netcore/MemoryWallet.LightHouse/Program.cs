using System;
using Akka.Actor;
using MemoryWallet.LightHouse;

namespace MemoryWallet.Lighthouse
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = DI.SystemActor;

            
            

            var logger = system.Log;

            
            logger.Info($"{system.Name}");

            var cluster = Akka.Cluster.Cluster.Get(system);
            
            Console.WriteLine($"{cluster.SelfAddress}");
            
            Console.ReadLine();

            cluster.Leave(cluster.SelfAddress);
            
            CoordinatedShutdown.Get(system)
                .Run(CoordinatedShutdown.ClrExitReason.Instance)
                .Wait();

            system.Log.Warning("Lighthouse Terminated");
            Environment.Exit(0);

        }
    }
}