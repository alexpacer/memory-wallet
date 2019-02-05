using Akka.Actor;
using MemoryWallet.Lib;
using MemoryWallet.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MemoryWallet.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ActorSelection _remoteManager;
        private readonly IActorRef _spkManager;

        public HomeController(SportsBookManagerSystemActorProvider sbkManager,
            ILogger<HomeController> logger,
            SportsBookManagerRemoteActorPRovider remoteManager)
        {
            _logger = logger;
            _remoteManager = remoteManager();
            _spkManager = sbkManager();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(LoginViewModel login)
        {
            // to do : return something
            ViewBag.UserName = login.Email;


            _spkManager.Tell("login");

            var playerLoginEvt = new PlayerLoginEvt
            {
                Email = login.Email
            };

            _remoteManager.Tell(playerLoginEvt);

            _logger.LogInformation($"User logging in: {login.Email} {_remoteManager.PathString}");
            return View();
        }
    }
}