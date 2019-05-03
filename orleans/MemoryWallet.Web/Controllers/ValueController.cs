using System.Threading.Tasks;
using MemoryWallet.GrainInterface;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace MemoryWallet.Web.Controllers
{
    [Route("api/[controller]")]
    public class ValueController : Controller
    {
        private readonly IClusterClient _client;

        public ValueController(IClusterClient client)
        {
            _client = client;
        }

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var grain = _client.GetGrain<IValueGrain>(1); 
            
            return Json(new { Message = "Ah...okay", Grain = await grain.GetValue() });
        }

        [Route("{id}")]
        public async Task<IActionResult> Value(long id)
        {
            var grain = _client.GetGrain<IValueGrain>(id);

            return Json(new { Message = "Retrieved Grain", GrainValue = await grain.GetValue() });
        }

        [Route("{id}/{newValue}")]
        public async Task<IActionResult> Value(long id, string newValue)
        {
            var grain = _client.GetGrain<IValueGrain>(id);
            await grain.SetValue(newValue);

            return Json(new { Message = "Updated Grain", GrainValue = await grain.GetValue() });
        }
    }
}