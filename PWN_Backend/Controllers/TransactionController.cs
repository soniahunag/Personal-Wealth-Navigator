using Microsoft.AspNetCore.Mvc;
using PWN.Shared;
using PWN_Backend.Services;

namespace PWN_Backend.Controllers
{

    /// <summary>  
    ///   連接前端API 的 Controller，提供手動觸發同步的端點，只處理跟帳務有關
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly MarketDataService _marketDataService;
        public TransactionController(MarketDataService service, AIService aIService)
        {
            _marketDataService = service;
        }
        [HttpGet("portfolio/{symbol}")]
        public async Task<IActionResult> GetPortfolio(string symbol)
        {
            var result = await _marketDataService.GetLatestPriceFromDbAsync(symbol);
            var holdings = 1000; // 暫時寫死，之後從 DB 抓
                                 // 如果沒抓到價格，回傳 404
            if (result == null)
            {
                return NotFound($"找不到標的 {symbol} 的價格資料");
            }

            // 成功路徑
            return Ok(new ProfolioAPIModel
            {
                CurrentHoldings = holdings,
                TotalMarketValue = holdings * result, // 注意：如果 result 是 decimal?，要加 .Value
                LastUpdated = DateTime.Now
            });
        }


    }
}
