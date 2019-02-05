using Akka.Actor;
using Akka.Event;
using Autofac;
using MemoryWallet.Lib;
using MemoryWallet.Lib.Model;
using MemoryWallet.Lib.Repository;
using MemoryWallet.Service.Actors;

namespace MemoryWallet.Service.Actor
{
    public class PlayerManagerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        
        public PlayerManagerActor(IPlayerRepository playerRepository)
        {
            ReceiveAsync<PlayerLoginEvt>(async l =>
            {
                var profile = await playerRepository.GetPlayer(l.Email);

                if (profile != null)
                {
                    Context.ActorOf(PlayerActor.Props(profile), $"player-{profile.Id}");
                    _log.Info($"player {profile.Id}::{profile.Name} Logged in, having balance of {profile.Balance}");                    
                }
            });

            Receive<PlayerLogoutEvt>(l =>
            {
                var player = Context.ActorSelection($"./player-{l.PlayerId}");
                player.Tell(PoisonPill.Instance);
                _log.Info($"player {l.PlayerId} Logged out");
            });

            // Testing only
            Receive<string>(l =>
            {
                _log.Info($"Received from remote actor {l}");
            });
        }
        
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => 
                new PlayerManagerActor(
                    DI.Container.Resolve<IPlayerRepository>()
                    ));
        }

        public class CreatePlayerEvt
        {
            public PlayerProfile Profile { get; set; }
        }

        public class PlayerLogoutEvt
        {
            public int PlayerId { get; set; }
        }
        
        private void HandleCreatePlayerEvt(CreatePlayerEvt evt)
        {
            
        }
    }
}