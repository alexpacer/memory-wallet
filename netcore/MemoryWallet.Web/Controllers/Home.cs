using Microsoft.AspNetCore.Mvc;

namespace MemoryWallet.Web.Controllers
{
    [Route("")]
    public class Home : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}