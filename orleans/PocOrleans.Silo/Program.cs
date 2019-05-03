using System;
using System.Threading.Tasks;
using Orleans.Hosting;
using Orleans;
using Microsoft.Extensions.Logging;
using PocOrleans.Grain;

namespace PocOrleans.Silo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var siloBuilder = new SiloHostBuilder()
                .AddMemoryGrainStorage("store1")
                .AddMemoryGrainStorage("players")
                .UseLocalhostClustering(serviceId: "orleans-poc-cluster")
                .ConfigureApplicationParts(c =>
                {
                    c.AddApplicationPart(typeof(ValueGrain).Assembly).WithReferences();
                    c.AddApplicationPart(typeof(PlayerGrain).Assembly).WithCodeGeneration();
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .UseDashboard();

            using (var host = siloBuilder.Build())
            {
                await host.StartAsync();
                
                Console.WriteLine("Orleans Silo Started...");    
                Console.ReadLine();
            }
        }
    }
}    