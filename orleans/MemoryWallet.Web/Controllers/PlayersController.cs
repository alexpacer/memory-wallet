using System.Threading.Tasks;
using MemoryWallet.GrainInterface;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace MemoryWallet.Web.Controllers
{
    [Route("api/[controller]")]
    public class PlayersController : Controller
    {
        private readonly IClusterClient _client;

        public PlayersController(IClusterClient client)
        {
            _client = client;
        }

        [Route("{id}")]
        public async Task<IActionResult> Index(long id)
        {
            var player = _client.GetGrain<IPlayerGrain>(id);

            var profile = await player.GetProfile();

            return Json(new
            {
                Balance = await player.GetBalance(),
                UserName = profile.UserName,
                Email = profile.Email
            });
        }

        [HttpPost("fundin")]
        public async Task<IActionResult> Deposit([FromBody]FundInModel req)
        {
            var player = _client.GetGrain<IPlayerGrain>(req.PlayerId);

            var profile = await player.GetProfile();

            await player.FundIn(req.Amt);

            return Json(new
            {
                BalanceBefore = profile.Balance,
                Balance = await player.GetBalance()
            });
        }

        [HttpPost("fundout")]
        public async Task<IActionResult> FundOut([FromBody]FundOutModel req)
        {
            var player = _client.GetGrain<IPlayerGrain>(req.PlayerId);

            var profile = await player.GetProfile();

            await player.FundOut(req.Amt);

            return Json(new
            {
                BalanceBefore = profile.Balance,
                Balance = await player.GetBalance()
            });
        }
    }

    public class FundOutModel
    {
        public long PlayerId { get; set; }
        public decimal Amt { get; set; }
    }

    public class FundInModel
    {
        public long PlayerId { get; set; }
        public decimal Amt { get; set; }
    }
}