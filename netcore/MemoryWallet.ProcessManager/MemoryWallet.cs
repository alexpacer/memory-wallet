using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using MemoryWallet.ProcessManager.Actor;

namespace MemoryWallet.ProcessManager
{
    public class MemoryWallet
    {
        private ActorSystem _system;
        private IActorRef _sportsbook;
        private ILoggingAdapter _logger;

        public void Start()
        {
            var sr = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "akka.conf"));
            var config = ConfigurationFactory.ParseString(sr);
            
            _system = ActorSystem.Create("MemoryWallet", config);
            _logger = _system.Log;

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