package awei.memory.wallet;

import akka.actor.AbstractActor;
import akka.actor.Props;
import akka.event.Logging;
import akka.event.LoggingAdapter;

import java.math.BigDecimal;

public class PlayerActor extends AbstractActor {

    private final LoggingAdapter log = Logging.getLogger(getContext().getSystem(), this);

    private PlayerProfile profile;

    public PlayerActor(String playerName){
        profile = new PlayerProfile(playerName);
    }

    public static class DepositEvt{

        private BigDecimal amt;

        public DepositEvt(BigDecimal amount) {
            amt = amount;
        }

        public DepositEvt(double amount) {
            amt = new BigDecimal(amount);
        }

        public BigDecimal getAmt(){ return amt; }
        public void setAmt(BigDecimal val){ amt = val; }
    }

    public static class WithdrawlEvt{
        private BigDecimal amt;

        public WithdrawlEvt(BigDecimal amount) {
            amt = amount;
        }

        public WithdrawlEvt(double amount) {
            amt = new BigDecimal(amount);
        }

        public BigDecimal getAmt(){ return amt; }
        public void setAmt(BigDecimal val){ amt = val; }
    }

    @Override
    public Receive createReceive() {
        return receiveBuilder()
                .match(
                  DepositEvt.class,
                  s -> {
                      if(s.amt.compareTo(new BigDecimal(0)) > 0){
                          BigDecimal newAmt = this.profile.getAmount().add(s.amt);
                          this.profile.setAmount(newAmt);
                          log.debug("deposited " + s.amt + ", new balance: " + this.profile.getAmount());
                      }
                  })
                .match(
                        WithdrawlEvt.class,
                        s -> {
                            if(s.amt.compareTo(new BigDecimal(0)) > 0 &&
                                    this.profile.getAmount().compareTo(s.amt) >= 0
                            ){
                                BigDecimal newAmt = this.profile.getAmount().subtract(s.amt);
                                this.profile.setAmount(newAmt);
                                log.debug("$" + s.amt + " withdrawl from account, new balance: " + this.profile.getAmount());
                            }
                        })
                .build();
    }

    public static Props Props(String playerName){
        return Props.create(PlayerActor.class, () -> new PlayerActor(playerName));
    }
}
