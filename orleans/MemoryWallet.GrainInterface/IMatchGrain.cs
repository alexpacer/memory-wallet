using System.Threading.Tasks;
using Orleans;

namespace MemoryWallet.GrainInterface
{
    public interface IMatchGrain : IGrainWithIntegerKey
    {
        Task Bet(decimal amt);
    }
}