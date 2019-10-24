using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;

namespace MemoryWallet.ProcessManager.Actor
{
    /// <summary>
    /// book of record of players registered in system
    /// </summary>
    public class PlayerBook : ReceivePersistentActor
    {
        private readonly ICollection<PlayerIdMap> _playerIdMaps;
        private ILoggingAdapter _logger = Context.GetLogger();

        public PlayerBook()
        {
            Recover<PlayerRegisteredEvt>(AddPlayer);

            Command<PlayerRegisteredEvt>(cmd => Persist(cmd, AddPlayer));
            Command<RecoveryCompleted>(completed => _logger.Info($"PlayerBook Recovery completed."));
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
    }

    public class PlayerIdMap
    {
        public long Id { get; set; }
        public string Email { get; set; }
    }
}