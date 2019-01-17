import akka.actor._
import akka.persistence._

trait WalletEvt{
  def amount: BigDecimal
}

case class DepositEvt(override val amount: BigDecimal) extends WalletEvt
case class WithdrawEvt(override val amount: BigDecimal) extends WalletEvt


class Player() extends Actor {

  var balance: BigDecimal = 0

  override def receive: Receive = {
    case dep: DepositEvt =>
      balance += dep.amount
      println(s"{$dep.amount} Received, thank you.")
    case wit: WithdrawEvt =>
      if(balance < wit.amount) throw new Error("Insufficient fund")
      else{
        balance -= wit.amount
      }
  }
}
