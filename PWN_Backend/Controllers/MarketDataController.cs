using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PWN_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketDataController : ControllerBase
    {
        private readonly Services.MarketDataService _marketDataService;
        public MarketDataController(Services.MarketDataService marketDataService)
        {
            _marketDataService = marketDataService;
        }
        [HttpPost("sync/0050")]
        public async Task<IActionResult> Sync0050()
        {
            var result = await _marketDataService.SyncStockPriceAsync("0050.TW");
            if (result) return Ok("同步成功");
            return BadRequest("同步失敗，請檢查 Log");
        }
    }
}
