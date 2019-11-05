using Akka.Actor;
using Akka.Event;
using MemoryWallet.Lib.Model;
using Microsoft.AspNetCore.SignalR;

namespace MemoryWallet.Web.Hub
{
    public class WebHub : ReceiveActor
    {
        private readonly IHubContext<PlayerHub> _playerHub;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public WebHub(IHubContext<PlayerHub> playerHub)
        {
            _playerHub = playerHub;
            
            Receive<PlayerAlreadyRegisteredEvt>(message =>
            {
//                await _playerHub.Clients.All.SendAsync("PlayerChannel", 
////            registerModel.Name, registerModel.Email);
                
                _playerHub.Clients.All.SendAsync(
                    "PlayerChannel", "player-already-existed", message.Email);

                _logger.Info($"=> hub {_playerHub}");

                var emailGroup = _playerHub.Clients.Group(message.Email);
                emailGroup?.SendAsync("notify", "player-already-existed");

                _logger.Info($"--> emailGroup => {emailGroup}");

                _logger.Info($"Published \"player-already-existed\" to signalr group '{message.Email}'");
            });
        }

        public static Props Props(IHubContext<PlayerHub> playerHub)
        {
            return Akka.Actor.Props.Create<WebHub>(() => new WebHub(playerHub));
        }
    }
}