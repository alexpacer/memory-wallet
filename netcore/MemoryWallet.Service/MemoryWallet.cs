using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using MemoryWallet.Service.Actor;
using MemoryWallet.Service.Actors;

namespace MemoryWallet.Service
{
    public class MemoryWallet
    {
        private ActorSystem _system;
        private IActorRef _sportsbook;

        public void Start()
        {
            var sr = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "akka.conf"));
            var config = ConfigurationFactory.ParseString(sr);
            
            _system = ActorSystem.Create("MemoryWallet", config);

            _sportsbook = _system.ActorOf(SportsBookActor.Props(), "Sportsbook");
        }

        public void Stop()
        {
            _sportsbook.GracefulStop(TimeSpan.FromSeconds(1));
            Console.WriteLine("MemoryWallet stopped.");

            _system.Terminate();
            Console.WriteLine("Actor System Terminated");
            
            Environment.Exit(0);
        }
    }
}