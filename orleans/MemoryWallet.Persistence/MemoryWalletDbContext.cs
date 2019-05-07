using MemoryWallet.Domain;
using Microsoft.EntityFrameworkCore;

namespace MemoryWallet.Persistence
{
    public class MemoryWalletDbContext : DbContext
    {
        public DbSet<Player> Player { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Player>()
                .ToTable("Players")
                .HasKey(x => x.Id);
        }
    }
}