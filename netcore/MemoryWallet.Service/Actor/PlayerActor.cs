using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using MemoryWallet.Lib.Model;

namespace MemoryWallet.Service.Actors
{
    public class PlayerActor : ReceivePersistentActor
    {
        private PlayerProfile _profile;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public PlayerActor(PlayerProfile profile)
        {
            _profile = profile;

            Recover<DepositEvt>(deposit =>
            {
                 HandleDeposit(deposit);
            });
            Recover<WithdrawalEvt>(w =>
            {
                HandleWithdrawal(w);
            });
            
            
            Command<RecoveryCompleted>(completed => _log.Info($"Recovery completed.") );

            Command<DepositEvt>(d => Persist(d, c=> HandleDeposit(d)));
            Command<WithdrawalEvt>(w => Persist(w, c=> HandleWithdrawal(w)));

            Command<GetBalanceEvt>(_ =>
            {
                Sender.Tell(this._profile.Balance, Self);
            });
        }

        private void HandleDeposit(DepositEvt depositEvt)
        {
            if (depositEvt.Amt > 0)
            {
                _profile.Balance += depositEvt.Amt;
            }
        }

        private void HandleWithdrawal(WithdrawalEvt withdrawalEvt)
        {
            if (withdrawalEvt.Amt <= _profile.Balance)
            {
                _profile.Balance -= withdrawalEvt.Amt;
            }
        }

        public static Props Props(PlayerProfile profile)
        {            
            return Akka.Actor.Props.Create(() => new PlayerActor(profile));
        }

        public override string PersistenceId => $"player-{_profile.Id}";


        public class DepositEvt
        {
            public decimal Amt { get; set; }
            public string Description { get; set; }
        }

        public class WithdrawalEvt
        {
            public decimal Amt { get; set; }
            public string Description { get; set; }
        }

        public class GetBalanceEvt
        {
        }
    }
}