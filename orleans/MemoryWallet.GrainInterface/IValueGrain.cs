using System.Threading.Tasks;
using Orleans;

namespace MemoryWallet.GrainInterface
{
    public interface IValueGrain : IGrainWithIntegerKey
    {
        Task<string> GetValue();

        Task SetValue(string value);
    }
}
