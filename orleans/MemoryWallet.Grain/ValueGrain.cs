using System.Threading.Tasks;
using MemoryWallet.GrainInterface;
using Orleans;
using Orleans.Providers;

namespace MemoryWallet.Grain
{
    [StorageProvider(ProviderName = "store1")]
    public class ValueGrain : Grain<ValueState>, IValueGrain
    {
        public async Task<string> GetValue()
        {
            await ReadStateAsync();

            if (State.Value != null) return State.Value;

            State.Value = "State Initialized";

            await WriteStateAsync();

            return State.Value;
        }

        public async Task SetValue(string value)
        {
            State.Value = value;

            await WriteStateAsync();
        }
    }
}