import akka.actor._
import akka.persistence._

trait WalletEvt {
  def amount: BigDecimal
}

object Player {

  case class DepositEvt(override val amount: BigDecimal) extends WalletEvt

  case class WithdrawEvt(override val amount: BigDecimal) extends WalletEvt

  def props(username: String): Props = Props(new Player(username))
}

class Player(username: String) extends Actor {

  import Player._

  var balance: BigDecimal = 0

  override def preStart(): Unit = {
    println(s"Player ${username} Logged in, initial balance ${balance}")
    super.preStart()
  }

  override def receive: Receive = {
    case dep: DepositEvt =>
      balance += dep.amount
      println(s"${dep.amount} Received, thank you.")
    case wit: WithdrawEvt =>
      if (balance < wit.amount) throw new Error("Insufficient fund")
      else {
        balance -= wit.amount
        println(s"${wit.amount} Withdrawn from account, ${balance} remains")
      }
  }
}


class Game() extends Actor {

  var id: BigInt = 0
  var name: String = ""

  def receive: Receive = ???

}
