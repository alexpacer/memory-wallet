using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MemoryWallet.GrainInterface;
using Orleans;
using Orleans.Providers;

namespace MemoryWallet.Grain
{
    [StorageProvider(ProviderName = "players")]
    public class PlayerGrain : Grain<PlayerState>, IPlayerGrain
    {
        private ICollection<IMatchGrain> _matches;

        public override Task OnActivateAsync()
        {
            var primaryKey = this.GetPrimaryKeyLong();
            if (string.IsNullOrEmpty(State.UserName))
            {
                // initialize player from some persistent storage other then Grain Silo
                // var user = _userRepo.GetUser(primaryKey);

                this.State.UserName = "alex.wei";
                this.State.Balance = 0;
                this.State.Email = "alex.wei@google.com";

                this.WriteStateAsync();
            }

            return base.OnActivateAsync();
        }

        public Task<ICollection<IMatchGrain>> GetMatchesBetted()
        {
            return Task.FromResult(_matches);
        }

        public Task Bet(IMatchGrain match, decimal amt)
        {
            return Task.CompletedTask;
        }

        public async Task FundIn(decimal amt)
        {
            if (amt > 0)
            {
                State.Balance += amt;
                await WriteStateAsync();
            }
            else
            {
                throw new Exception("Cannot found in 0 or negative amount.");
            }
        }

        public async Task FundOut(decimal amt)
        {
            await ReadStateAsync();

            if (amt > State.Balance)
            {
                throw new Exception(
                    $"Insufficient Balance (Withdrawing {amt} out of {State.Balance})");
            }
            else
            {
                State.Balance -= amt;
                await WriteStateAsync();
            }
        }

        public Task<decimal> GetBalance()
        {
            return Task.FromResult(State.Balance);
        }

        public Task<PlayerState> GetProfile()
        {
            return Task.FromResult(State);
        }
    }
}