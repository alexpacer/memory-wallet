import Match.SetDetailEvt
import Player._
import Sportsbook.CreateMatchEvt
import akka.actor.{ActorSystem, Props}
import org.joda.time.DateTime

object Main {
  def main(args: Array[String]): Unit = {
    println("Starting Memory wallet...")
    val system = ActorSystem("memory-wallet")
    val player = system.actorOf(Player.props("Alex"), "PlayerAlex")
    player.tell(DepositEvt(100), null)
    player.tell(WithdrawEvt(10), null)

    val sportsbook = system.actorOf(Props[Sportsbook], "Sportsbook")
    val match1 = system.actorOf(Match.props(), name = "match1")

    val match1Detail = new MatchDetail{
      val name = "A vs B"
      val kickoff = new DateTime(2019, 1, 1, 1, 1)
    }
    val evt = SetDetailEvt(match1Detail)

    match1.tell(evt, null)

  }
}
