using System;
using Akka.Actor;
using Akka.Configuration;
using MemoryWallet.Lib;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MemoryWallet.ProcessManager
{
    public static class DI
    {
        public delegate ActorSelection HubProvider();

        public static IServiceProvider Provider => _provider.Value;

        private static readonly Lazy<IServiceProvider> _provider = new Lazy<IServiceProvider>(() =>
        {
            var c = new ServiceCollection();
            c.BuildCommonDependency();

            c.AddSingleton<ActorSystem>(s => SystemActor);
            c.AddTransient<HubProvider>(s => () => s.GetService<ActorSystem>().ActorSelection("/user/hub"));

            return c.BuildServiceProvider();
        });

        public static ActorSystem SystemActor => _systemActor.Value;

        private static Lazy<ActorSystem> _systemActor => new Lazy<ActorSystem>(() =>
        {
            // Initialize logger so System Actor could pick it up
            Provider.GetService<ILogger>();
            
            var baseFile = Provider.GetService<IBaseFileFactory>();
            
            var confFile = baseFile.ReadRelative("process-manager.hocon");

            var conf = ConfigurationFactory.ParseString(confFile);

            var system = ActorSystem.Create("sbk", conf);
            
            return system;
        });
    }
}