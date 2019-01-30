using Akka.Actor;
using Akka.Event;
using MemoryWallet.Service.Model;

namespace MemoryWallet.Service.Actors
{
    public class PlayerManagerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        
        public PlayerManagerActor()
        {
            Receive<PlayerLoginEvt>(l =>
            {
                Context.ActorOf(PlayerActor.Props(l.Profile), $"player-{l.PlayerId}");
                _log.Info($"player {l.Profile.Name} Logged in");
            });

            Receive<PlayerLogoutEvt>(l =>
            {
                var player = Context.ActorSelection($"./player-{l.PlayerId}");
                player.Tell(PoisonPill.Instance);
                _log.Info($"player {l.PlayerId} Logged out");
            });
            
            // Console.WriteLine(playerManager);
        }
        
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new SportsBookActor());
        }

        public class CreatePlayerEvt
        {
            public PlayerProfile Profile { get; set; }
        }

        public class PlayerLoginEvt
        {
            public int PlayerId { get; set; }
            public PlayerProfile Profile { get; set; }
        }

        public class PlayerLogoutEvt
        {
            public int PlayerId { get; set; }
        }
        
        private void HandleCreatePlayerEvt(CreatePlayerEvt evt)
        {
            
        }
        
        private void HandlePlayerLoginEvt(PlayerLoginEvt evt)
        {
            
        }
    }
}