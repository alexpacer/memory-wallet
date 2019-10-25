using Akka.Actor;
using MemoryWallet.Lib;
using MemoryWallet.Lib.Model;
using MemoryWallet.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MemoryWallet.Web.Controllers
{
    [Route("player")]
    public class Player : Controller
    {
        private readonly ILogger _logger;
        private readonly ActorSelection _playerManagers;
        
        public Player(Startup.PlayerManagerProvider provider, 
            ILogger logger)
        {
            _logger = logger;
            _playerManagers = provider();
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
            
            _logger.Warning($"telling: {_playerManagers}");
            _playerManagers.Tell(newPlayer);
            
            return Redirect("/");
        }
        
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost("login")]
        public IActionResult DoLogin(PlayerLoginModel req)
        {
            var loginCmd = new PlayerLoginEvt(req.Email);
            _playerManagers.Tell(loginCmd);

            return Redirect("/player/logging-in");
        }

        [HttpGet("logging-in")]
        public IActionResult LoggingIn()
        {
            return View();
        }
    }
}