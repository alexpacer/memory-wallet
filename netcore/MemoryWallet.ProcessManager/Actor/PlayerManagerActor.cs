using System;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using IdGen;
using MemoryWallet.Lib;
using MemoryWallet.Lib.Model;
using MemoryWallet.Lib.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace MemoryWallet.ProcessManager.Actor
{
    public class PlayerManagerActor : ReceiveActor, IWithUnboundedStash
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IdGenerator _idGenerator;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly ActorSelection _playerBookProxy = Context.System.ActorSelection("/user/playerbook-proxy");
        
        private IActorRef _playerManagerBroadcaster;

        private PlayerIdMap _currentLookupPlayer;
        
        private int OutstandingPlayerActorLookupAcks { get; set; }

        public IStash Stash { get; set; }

        public PlayerManagerActor(IPlayerRepository playerRepository, IdGenerator idGenerator)
        {
            _playerRepository = playerRepository;
            _idGenerator = idGenerator;
            
            Ready();
        }

        protected override void PreStart()
        {
            // Get local broadcaster instance as my child node 
            _playerManagerBroadcaster = Context.Child("broadcaster").Equals(ActorRefs.Nobody)
                ? Context.ActorOf(Akka.Actor.Props.Empty.WithRouter(FromConfig.Instance), "broadcaster")
                : Context.Child("broadcaster");
            
            _logger.Info($"[{Self.Path}]: broadcaster: {_playerManagerBroadcaster.Path}");
        }


        private void Ready()
        {
            Receive<HelloWorld>(s => { _logger.Info($"{Self.Path} received {s.Message}"); });

            Receive<PlayerLoginEvt>(l =>
            {
                _playerManagerBroadcaster.Tell(new HelloWorld("Hello!!"));
                
                _playerBookProxy.Tell(new PlayerBook.FindPlayerIdReq(l.Email));
                Become(LookingUpPlayer);
            });

            Receive<CreatePlayerEvt>(c =>
            {
                var playerId = _idGenerator.CreateId();

                var newProfile = new PlayerProfile(id: playerId, name: c.Name, email: c.Email);

                var newPlayerCmd = new PlayerBook.PlayerRegisteredEvt(playerId, c.Name, c.Email);

                _playerBookProxy.Tell(newPlayerCmd);

                _playerRepository.CreatePlayer(newProfile);
            });

            Receive<PlayerLogoutEvt>(l =>
            {
                var player = Context.ActorSelection($"./player-{l.PlayerId}");
                player.Tell(PoisonPill.Instance);
                _logger.Info($"player {l.PlayerId} Logged out");
            });
        }

        private void LookingUpPlayer()
        {
            // Spend at most 10 seconds to lookup a player
            Context.SetReceiveTimeout(null);
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(10.0));
            
            Receive<PlayerLoginEvt>(e => Stash.Stash());

            Receive<PlayerIdMap>(map =>
            {
                // put in local state
                _currentLookupPlayer = map;
                
                // create player actor by checking if player actor is around
                _playerManagerBroadcaster.Tell(new DoYouHaveThisPlayerReq(map));
                
                var managerMembers = _playerManagerBroadcaster.Ask<Routees>(
                    new GetRoutees()).Result.Members.ToList();
                OutstandingPlayerActorLookupAcks = managerMembers.Count();
                
                _logger.Info($"OutstandingPlayerActorLookupAcks: {OutstandingPlayerActorLookupAcks}");
                
            });

            Receive<DoYouHaveThisPlayerReq>(HandlePlayerLookupRequest);
            Receive<PlayerFoundEvt>(HandlePlayerFoundEvt);
            
            Receive<PlayerNotFoundEvt>(evt =>
            {
                if (evt.Player.Id == _currentLookupPlayer.Id)
                {
                    // Indicates this node is the one who sent out request to lookup player
                    OutstandingPlayerActorLookupAcks--;

                    if (OutstandingPlayerActorLookupAcks <= 0)
                    {
                        // Since no one has this player (including myself) 
                        // We will create this player actor here
                        
                        var player = Context.ActorOf(
                            PlayerActor.Props(evt.Player), evt.Player.Id.ToString());

                        _playerManagerBroadcaster.Tell(new PlayerFoundEvt(evt.Player, player));

                        _logger.Debug($"Player {player.Path} created in {Self.Path}");
                    }
                }
            });

            Receive<ReceiveTimeout>(timeout =>
            {
                _logger.Debug(
                    $"[{Self.Path}]: Waited for 10secs without Playerbook response, player may not have registered..");
                BecomeReady();
            });
        }

        private void HandlePlayerLookupRequest(DoYouHaveThisPlayerReq req)
        {
            _logger.Warning($"[{Self.Path}]: Received {req.Player.Id}-{req.Player.Email} lookup req");
            // Find if this player id is one of my child node
            var myPlayer = Context.Child($"{req.Player.Id}");
            
            // Found player as my child node
            if (!myPlayer.Equals(ActorRefs.Nobody))
            {
                _playerManagerBroadcaster.Tell(new PlayerFoundEvt(req.Player, myPlayer));
                _logger.Debug($"Player found in cluster {myPlayer.Path}");
                return;
            }

            _playerManagerBroadcaster.Tell(new PlayerNotFoundEvt(req.Player));
        }

        private void HandlePlayerFoundEvt(PlayerFoundEvt e)
        {
            // we found the actor!
            BecomeReady();
        }

        private void BecomeReady()
        {
            Become(Ready);
            Context.SetReceiveTimeout(null);
            Stash.UnstashAll();
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
    }

}