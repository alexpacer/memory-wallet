using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace PocOrleans.GrainInterface
{
    public interface IPlayerGrain : IGrainWithIntegerKey
    {
        Task<ICollection<IMatchGrain>> GetMatchesBetted();

        Task Bet(IMatchGrain match, decimal amt);

        Task FundIn(decimal amt);

        Task FundOut(decimal amt);

        Task<decimal> GetBalance();

        Task<PlayerState> GetProfile();
    }
}