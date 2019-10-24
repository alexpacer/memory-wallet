using Akka.Routing;

namespace MemoryWallet.Lib.Model
{
    public class ICreatePlayerEvt
    {
        public string Name { get; }
        public string Email { get; }
    }
    
    public class CreatePlayerEvt : ICreatePlayerEvt, IConsistentHashable
    {
        public CreatePlayerEvt(string name, string email)
        {
            Name = name;
            Email = email;
        }
        
        public string Name { get; }
        public string Email { get; }

        public object ConsistentHashKey => Email;
    }

}