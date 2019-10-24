using System;
using Akka.Actor;
using Akka.Configuration;
using MemoryWallet.Lib;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MemoryWallet.LightHouse
{
    public class DI
    {
        public static ActorSystem SystemActor => _systemActor.Value;

        private static Lazy<ActorSystem> _systemActor => new Lazy<ActorSystem>(() =>
        {
            var logger = Provider.GetService<ILogger>();
            var baseFile = Provider.GetService<IBaseFileFactory>();

            var confFile = baseFile.ReadRelative("lighthouse.hocon");

            var conf = ConfigurationFactory.ParseString(confFile);

            return ActorSystem.Create("sbk", conf);
        });


        public static IServiceProvider Provider => _provider.Value;

        private static readonly Lazy<IServiceProvider> _provider = new Lazy<IServiceProvider>(() =>
        {
            var c = new ServiceCollection();
            c.BuildCommonDependency();

            return c.BuildServiceProvider();
        });
    }
}