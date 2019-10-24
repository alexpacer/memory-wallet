using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using MemoryWallet.Lib.Model;

namespace MemoryWallet.ProcessManager.Actor
{
    public class PlayerActor : ReceivePersistentActor
    {
        private readonly PlayerProfile _profile;
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public PlayerActor(PlayerProfile profile)
        {
            _profile = profile;

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

        public static Props Props(PlayerProfile profile)
        {            
            return Akka.Actor.Props.Create(() => new PlayerActor(profile));
        }

        public override string PersistenceId => $"player-{_profile.Id}";
        
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