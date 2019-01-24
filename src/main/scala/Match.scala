import Match.{AddBetTypeEvt, SetDetailEvt}
import akka.actor.{Actor, Props}
import org.joda.time.DateTime

object Match {
  case class AddBetTypeEvt(betType:BetType)
  case class SetDetailEvt(matchDetail: MatchDetail)
  def props(): Props = Props[Match]
}

class Match() extends Actor {
  var betTypes: List[BetType] = List()

  var name:String = _
  var kickoff:DateTime = _

  def receive: Receive  = {
    case abt:AddBetTypeEvt =>
      betTypes :+ abt.betType
    case d:SetDetailEvt =>
      name = d.matchDetail.name
      kickoff = d.matchDetail.kickoff
      print(s"Event created $name, $kickoff")
  }
}

trait MatchDetail{
  val name: String
  val kickoff: DateTime
}

trait BetType {
  def name: String
  def selections: List[Selection]
}

trait Selection {
  def name: String
  def odds: BigDecimal
}



