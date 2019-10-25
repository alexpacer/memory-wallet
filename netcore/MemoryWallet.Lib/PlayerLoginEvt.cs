using Akka.Routing;

namespace MemoryWallet.Lib
{
    public class PlayerLoginEvt : IConsistentHashable
    {
        public PlayerLoginEvt(string email)
        {
            Email = email;
        }

        public string Email { get; }
        public object ConsistentHashKey => Email;
    }
}