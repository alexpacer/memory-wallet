using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace MemoryWallet.Web.Actors
{
    public class SportsBookManagerActor : ReceiveActor
    {
        public SportsBookManagerActor(ILogger<IActorRef> logger)
        {
            Receive<string>(s =>
            {
                switch (s)
                {
                    case "login":
                        logger.LogInformation($"Actor: {s}");
                        break;
                }
                
            });
        }
        
        public static Props Props(ILogger<IActorRef> logger)
        {
            return Akka.Actor.Props.Create(() => new SportsBookManagerActor(logger));
        }
    }
}