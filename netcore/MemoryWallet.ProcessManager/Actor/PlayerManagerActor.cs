using System;
using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Event;
using Akka.Routing;
using IdGen;
using MemoryWallet.Lib;
using MemoryWallet.Lib.Model;
using MemoryWallet.Lib.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace MemoryWallet.ProcessManager.Actor
{
    public class PlayerManagerActor : ReceiveActor
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IdGenerator _idGenerator;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public PlayerManagerActor(IPlayerRepository playerRepository, IdGenerator idGenerator)
        {
            _playerRepository = playerRepository;
            _idGenerator = idGenerator;
            Receive<HelloWorld>((s) =>
            {
                _logger.Info($"{Self.Path} received {s.Message}");
            });

            Receive<PlayerLoginEvt>(l =>
            {
                var profile = _playerRepository.GetPlayer(l.Id);

                if (profile != null)
                {
                    Context.ActorOf(PlayerActor.Props(profile), $"{profile.Id}");
                    _logger.Info($"player {profile.Id}::{profile.Name} Logged in, having balance of {profile.Balance}");
                }
            });

            Receive<CreatePlayerEvt>(c =>
            {
                var playerId = _idGenerator.CreateId();
                
                
//                var playerbook = Context.System.ActorOf(ClusterSingletonProxy.Props(
//                        singletonManagerPath: "/user/playerbook",
//                        settings: ClusterSingletonProxySettings.Create(Context.System)
//                            .WithRole("player-manager")),
//                    name: "playerbook-proxy");
//                
                

                    var newProfile = new PlayerProfile(id: playerId, name: c.Name, email: c.Email);
                
//                _logger.Info($"Creating Player {newProfile}");


                var newPlayerCmd = new PlayerBook.PlayerRegisteredEvt(playerId, c.Name, c.Email);
                var playerbook = Context.System.ActorSelection("/user/playerbook");
                playerbook.Tell(newPlayerCmd);

                _playerRepository.CreatePlayer(newProfile);
            });

            Receive<PlayerLogoutEvt>(l =>
            {
                var player = Context.ActorSelection($"./player-{l.PlayerId}");
                player.Tell(PoisonPill.Instance);
                _logger.Info($"player {l.PlayerId} Logged out");
            });
        }

        public static Props Props()
        {
            var playerRepo = DI.Provider.GetService<IPlayerRepository>();
            var idGen = DI.Provider.GetService<IdGenerator>();

            return Akka.Actor.Props.Create(() =>
                new PlayerManagerActor(playerRepo, idGen));
        }
        
        public class PlayerLogoutEvt
        {
            public int PlayerId { get; set; }
        }

        public class HelloWorld : IConsistentHashable
        {
            public HelloWorld(string message)
            {
                Message = message;
                ConsistentHashKey = Guid.NewGuid();
            }

            public string Message { get; }
            public object ConsistentHashKey { get; private set; }
        }

        private void HandleCreatePlayerEvt(CreatePlayerEvt evt)
        {
        }
    }
}