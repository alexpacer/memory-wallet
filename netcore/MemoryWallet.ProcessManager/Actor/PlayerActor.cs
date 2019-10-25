using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using MemoryWallet.Lib.Model;

namespace MemoryWallet.ProcessManager.Actor
{
    public class PlayerActor : ReceivePersistentActor
    {
        private readonly PlayerProfile _profile;
        private readonly PlayerIdMap _playerIdMap;
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public PlayerActor(PlayerIdMap playerIdMap)
        {
            _playerIdMap = playerIdMap;
            
            // TODO: We might want to store some more interesting information of this player
            // they can be stored in other storage rather then in memory so fuck that for now
            _profile = new PlayerProfile(_playerIdMap.Id, null, _playerIdMap.Email);

            Recover<DepositEvt>(d => _profile.Deposit(d.Amt));
            Recover<WithdrawalEvt>(w => _profile.Withdrawal(w.Amt));
            
            
            Command<RecoveryCompleted>(completed => _log.Info($"Recovery completed.") );

            Command<DepositEvt>(d => Persist(d, c=> _profile.Deposit(c.Amt)));
            Command<WithdrawalEvt>(w => Persist(w, c=> _profile.Deposit(c.Amt)));

            Command<GetBalanceEvt>(_ =>
            {
                Sender.Tell(this._profile.Balance, Self);
            });
        }

        public static Props Props(PlayerIdMap profile)
        {            
            return Akka.Actor.Props.Create(() => new PlayerActor(profile));
        }

        public override string PersistenceId => $"player-{_playerIdMap.Id}";
        
        public class DepositEvt
        {
            public DepositEvt(decimal amt, string description)
            {
                Amt = amt;
                Description = description;
            }

            public decimal Amt { get; }
            public string Description { get; }
        }

        public class WithdrawalEvt
        {
            public WithdrawalEvt(decimal amt, string description)
            {
                Amt = amt;
                Description = description;
            }

            public decimal Amt { get; }
            public string Description { get; }
        }

        public class GetBalanceEvt
        {
        }
    }
}