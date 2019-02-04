using System;
using Akka.Actor;
using Akka.Event;
using IdGen;
using MemoryWallet.Service.Model;

namespace MemoryWallet.Service.Actors
{
    public class SportsBookActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public SportsBookActor()
        {

            Receive<string>(r =>
            {
                _log.Info($"SportsBookActor received: {r}, {Sender.Path.Address}");
            });
            
            var playerManager = Context.ActorOf(PlayerManagerActor.Props(), "PlayerManager");

//            playerAlex.Tell(new PlayerActor.DepositEvt {Amt = 100, Description = "Initial balance"}, Self);
//            playerAlex.Tell(new PlayerActor.DepositEvt {Amt = 2000, Description = "Found 2 notes on floor."}, Self);
//            playerAlex.Tell(new PlayerActor.DepositEvt {Amt = 100000, Description = "If I am a rich man, Lalalalalalalalalalalalala~"}, Self);

//            playerManager.Ask<decimal>(new PlayerActor.GetBalanceEvt())
//                .ContinueWith(c => Console.WriteLine("-----" + c.Result));

            var idGenerator = new IdGenerator(0);
           
            var alexProfile = new PlayerProfile {Name = "Alex", Id = idGenerator.CreateId()};
            playerManager.Tell(new PlayerManagerActor.PlayerLoginEvt {Profile = alexProfile});


//            var playerAlex = Context.ActorSelection($"/user/MemoryWallet/Sportsbook/PlayerManager/player-{alexProfile.Id}");
            
            

//            playerAlex.Tell(new PlayerActor.DepositEvt {Amt = 100, Description = "Initial balance"}, Self);
//            playerAlex.Tell(new PlayerActor.DepositEvt {Amt = 2000, Description = "Found 2 notes on floor."}, Self);
//            playerAlex.Tell(new PlayerActor.DepositEvt {Amt = 100000, Description = "If I am a rich man, Lalalalalalalalalalalalala~"}, Self);
//            playerAlex.Tell(new PlayerActor.WithdrawalEvt {Amt = Convert.ToDecimal(22.0), Description = "十元買早餐~八元買豆乾"}, Self);
//
//            playerAlex.Ask<decimal>(new PlayerActor.GetBalanceEvt())
//                .ContinueWith(c => Console.WriteLine("balance: $" + c.Result));
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new SportsBookActor());
        }
    }
}