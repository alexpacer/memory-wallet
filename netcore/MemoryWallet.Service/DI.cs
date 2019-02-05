using System;
using System.IO;
using Autofac;
using MemoryWallet.Lib.Repository;
using Microsoft.Extensions.Configuration;

namespace MemoryWallet.Service
{
    public static class DI
    {
        public static IContainer Container => _container.Value;

        private static readonly Lazy<IContainer> _container = new Lazy<IContainer>(() =>
        {
            var builder = new ContainerBuilder();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            builder.Register<IConfiguration>(context => config);

            builder.RegisterType<PlayerRepository>().As<IPlayerRepository>().AsSelf();
            
            return builder.Build();
        });
    }
}