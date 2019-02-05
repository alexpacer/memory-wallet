using System.Threading.Tasks;
using IdGen;
using MemoryWallet.Lib.Model;

namespace MemoryWallet.Lib.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        async Task<PlayerProfile> IPlayerRepository.GetPlayer(string email)
        {
            var idGenerator = new IdGenerator(0);
           
            var alexProfile = new PlayerProfile {Name = "Alex", Id = idGenerator.CreateId(), Balance = 3000 };

            return await Task.FromResult(alexProfile);
        }
    }

    public interface IPlayerRepository
    {
        Task<PlayerProfile> GetPlayer(string email);
    }
}