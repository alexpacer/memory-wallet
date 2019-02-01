using Akka.Actor;
using MemoryWallet.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MemoryWallet.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IActorRef _spkManager;

        public HomeController(SportsBookManagerSystemActorProvider spkManager, ILogger<HomeController> logger)
        {
            _logger = logger;
            _spkManager = spkManager();
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Login(LoginViewModel login)
        {
            // to do : return something
            ViewBag.UserName = login.Email;
            
            _logger.LogInformation($"User logging in: {login.Email}");
            
            _spkManager.Tell("login");
            
            return View();
        }
    }
}