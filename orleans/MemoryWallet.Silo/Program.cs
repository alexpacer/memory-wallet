using System;
using System.Threading.Tasks;
using MemoryWallet.Grain;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;

namespace MemoryWallet.Silo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var siloBuilder = new SiloHostBuilder()
                .AddMemoryGrainStorage("store1")
                .AddAdoNetGrainStorage("players", opt =>
                {
                    opt.Invariant = "System.Data.SqlClient";
                    opt.ConnectionString =
                        "Data Source=localhost;initial catalog=MemoryWallet;integrated security=false;User ID=sa;Password=@bc123456;";
//                        "Data Source=pulsars03\\TEST2016;initial catalog=MemoryWallet;integrated security=false;User ID=sa;Password=@bc123456;"
                })
//                .AddMemoryGrainStorage("players")
                .UseLocalhostClustering(serviceId: "orleans-poc-cluster")
                .ConfigureApplicationParts(c =>
                {
                    c.AddApplicationPart(typeof(ValueGrain).Assembly).WithReferences();
                    c.AddApplicationPart(typeof(PlayerGrain).Assembly).WithReferences();
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .UseDashboard(d =>
                {
                    d.Port = 3000;
                });    

            using (var host = siloBuilder.Build())
            {
                await host.StartAsync();
                
                Console.WriteLine("Orleans Silo Started...");    
                Console.ReadLine();
            }
        }
    }
}    