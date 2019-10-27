using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using MemoryWallet.Lib.Model;

namespace MemoryWallet.ProcessManager.Actor
{
    /// <summary>
    /// book of record of players registered in system
    /// </summary>
    public class PlayerBook : ReceivePersistentActor
    {
        private readonly ICollection<PlayerIdMap> _playerIdMaps;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public PlayerBook()
        {
            _playerIdMaps = new List<PlayerIdMap>();
            
            Recover<PlayerRegisteredEvt>(AddPlayer);

            Command<PlayerRegisteredEvt>(cmd =>
            {
                // Check if user exists before persist the event

                if (_playerIdMaps.FirstOrDefault(p => p.Email == cmd.Email) != null)
                {
                    Sender.Tell(new PlayerAlreadyRegisteredEvt(cmd.Email));
                    return;
                }

                Persist(cmd, AddPlayer);
            });
            Command<RecoveryCompleted>(completed => _logger.Warning($"PlayerBook Recovery completed."));
            Command<string>(s =>
            {
                _logger.Info($"string {s} received in playerbook");
            });

            Command<FindPlayerIdReq>(q =>
            {
                _logger.Info($"[{Self.Path}]:Looking up for player {q.Email}");
                
                var playerIdMap = _playerIdMaps.FirstOrDefault(p => p.Email == q.Email);
                
                if (playerIdMap != null)
                {
                    _logger.Info($"[{Self.Path}]:Player {q.Email} found! {playerIdMap.Id}:{playerIdMap.Email}");
                    Sender.Tell(playerIdMap);
                }
                else
                {
                    _logger.Info($"[{Self.Path}]:Player {q.Email} not found");                    
                }
            });
        }

        private void AddPlayer(PlayerRegisteredEvt playerIdMap)
        {
            _playerIdMaps.Add(new PlayerIdMap
            {
                Email = playerIdMap.Email,
                Id = playerIdMap.Id
            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<PlayerBook>(() => new PlayerBook());
        }

        public override string PersistenceId => $"playerbook";

        public class PlayerRegisteredEvt
        {
            public PlayerRegisteredEvt(long id, string name, string email)
            {
                Id = id;
                Name = name;
                Email = email;
            }

            public long Id { get; private set; }
            public string Name { get; private set; }
            public string Email { get; private set; }
        }

        public class FindPlayerIdReq
        {
            public FindPlayerIdReq(string email)
            {
                Email = email;
            }

            public string Email { get; }
        }
    }
    
}