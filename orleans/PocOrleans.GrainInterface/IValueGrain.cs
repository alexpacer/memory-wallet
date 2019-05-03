using System.Threading.Tasks;
using Orleans;

namespace PocOrleans.GrainInterface
{
    public interface IValueGrain : IGrainWithIntegerKey
    {
        Task<string> GetValue();

        Task SetValue(string value);
    }
}
