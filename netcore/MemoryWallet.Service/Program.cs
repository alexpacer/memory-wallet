using System;
using Topshelf;

namespace MemoryWallet.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello from MemoryWallet!");

            HostFactory.Run(x =>
            {
                x.RunAsLocalService();
                x.StartAutomaticallyDelayed();
                x.SetDescription("MemoryWallet Demo Code");
                x.SetDisplayName("Memory Wallet");
                x.SetServiceName("MemoryWallet");
                x.Service<MemoryWallet>(sc =>
                {
                    sc.ConstructUsing(name => new MemoryWallet());
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());
                });
            });
        }
    }
}