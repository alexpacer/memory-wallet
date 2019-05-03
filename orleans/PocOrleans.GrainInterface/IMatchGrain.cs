using System.Threading.Tasks;
using Orleans;

namespace PocOrleans.GrainInterface
{
    public interface IMatchGrain : IGrainWithIntegerKey
    {
        Task Bet(decimal amt);
    }
}