package awei.memory.wallet;

import akka.actor.ActorRef;
import akka.actor.ActorSystem;

public class Main {

    public static void main(String[] args){

        System.out.println("Hello World");

        final ActorSystem system = ActorSystem.create("system");

        ActorRef player = system.actorOf(PlayerActor.Props("Alex"), "PlayerAlex");

        player.tell(new PlayerActor.DepositEvt(100.00), ActorRef.noSender());
        player.tell(new PlayerActor.WithdrawlEvt(10.00), ActorRef.noSender());
    }

}

