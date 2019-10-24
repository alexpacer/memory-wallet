using Akka.Actor;
using MemoryWallet.Lib.Model;
using MemoryWallet.ProcessManager.Actor;
using MemoryWallet.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MemoryWallet.Web.Controllers
{
    [Route("player")]
    public class Player : Controller
    {
        private readonly ILogger _logger;
        private readonly ActorSelection _actorSelection;
//        private readonly IActorRef _playerBookProxy;
        
        public Player(Startup.PlayerManagerProvider provider, 
//            Startup.PlayerBookProxyProvider playerBookProxyProvider,
            ILogger logger)
        {
            _logger = logger;
//            _playerBookProxy = playerBookProxyProvider();
            _actorSelection = provider();
        }
        
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost("register")]
        public IActionResult RegisterUser(RegisterPlayerModel req)
        {
            var newPlayer = new CreatePlayerEvt(req.Name, req.Email);
            
            _logger.Warning($"telling: {_actorSelection}");
            _actorSelection.Tell(newPlayer);
            
            return Redirect("/");
        }
    }
}