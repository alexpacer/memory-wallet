using MemoryWallet.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MemoryWallet.Web.Startups
{
    public static class DbContextConfig
    {
        public static void RegisterSqlContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<MemoryWalletDbContext>(ctx => new MemoryWalletDbContext());
        }
    }
}