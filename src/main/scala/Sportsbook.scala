import akka.actor.Actor

object Sportsbook {
  case class CreateMatchEvt(m:Match)
}

class Sportsbook() extends Actor {
  import Sportsbook._

  var matches: List[Match] = List()

  def receive: Actor.Receive = {
    case cme: CreateMatchEvt =>
      matches :+ cme
      println(matches.map(m => m.name))
      println(s"number of matches: ${matches.length}")

  }
}


