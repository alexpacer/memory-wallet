import akka.actor.{ActorSystem, Props}

object Main {

  def main(args: Array[String]): Unit = {
    println("Starting Memory wallet...")

    val system = ActorSystem("memory-wallet")

    val player = system.actorOf(Props[Player], "PlayerAlex")

    player.tell(new DepositEvt(100), null)

  }
}
