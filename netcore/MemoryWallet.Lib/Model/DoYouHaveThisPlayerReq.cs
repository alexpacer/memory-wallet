using Akka.Actor;
using Newtonsoft.Json;

namespace MemoryWallet.Lib.Model
{
    /// <summary>
    /// Asking PlayerManager to check their child node 
    /// </summary>
    public class DoYouHaveThisPlayerReq
    {
        [JsonConstructor]
        public DoYouHaveThisPlayerReq(PlayerIdMap player)
        {
            Player = player;
        }

        public PlayerIdMap Player { get; }
    }

    public class PlayerNotFoundEvt
    {
        [JsonConstructor]
        public PlayerNotFoundEvt(PlayerIdMap player)
        {
            Player = player;
        }

        public PlayerIdMap Player { get; }
    }
    
    
    public class PlayerFoundEvt
    {
        public PlayerFoundEvt(PlayerIdMap player, IActorRef playerActor)
        {
            Player = player;
            PlayerActor = playerActor;
        }

        public PlayerIdMap Player { get; }
        public IActorRef PlayerActor { get; }
    }
}