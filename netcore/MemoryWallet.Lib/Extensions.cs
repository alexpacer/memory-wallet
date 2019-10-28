using System;
using System.Text.Json;
using IdGen;
using MemoryWallet.Lib.Model;
using MemoryWallet.Lib.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MemoryWallet.Lib
{
    public static class Extensions
    {
        public static void BuildCommonDependency(this IServiceCollection c)
        {
            IBaseFileFactory baseFileFactory = new BaseFileFactory(AppContext.BaseDirectory);
            c.AddTransient<IBaseFileFactory>(ctx => baseFileFactory);

            var config = new ConfigurationBuilder()
                .AddJsonFile(
                    "appsettings.json",
                    false, true)
                .Build();
            c.AddTransient<IConfiguration>(srv => config);

            c.AddTransient<IPlayerRepository, PlayerRepository>();

            c.AddTransient<ILogger>(l =>
            {
                Serilog.Log.Logger = new LoggerConfiguration()
                    .ReadFrom
                    .Configuration(config)
                    .CreateLogger();

                return Serilog.Log.Logger;
            });

            c.AddTransient<IdGenerator>(id =>
                new IdGenerator(456, new DateTime(1970, 1, 1), MaskConfig.Default));

            c.AddTransient<IFileDirHelper, FileDirDirHelper>();
        }

        public static string ToJsonString(this PlayerProfile o)
        {
            return JsonSerializer.Serialize(o);
        }

        public static PlayerProfile ConvertToPlayerProfile(string jsonString)
        {
            return JsonSerializer.Deserialize<PlayerProfile>(jsonString);
        }
        
    }
}