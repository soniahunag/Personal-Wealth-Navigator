using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime.Text;
using PWN.Shared;
using PWN_Backend.Services;

namespace PWN_Backend.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AIService _aiService;
        private readonly MarketDataService _marketDataService;
        public TransactionController(MarketDataService service, AIService aIService)
        {
            _marketDataService = service;
            _aiService = aIService; 
        }
        [HttpGet("protfolio/{symbol}")]
        public async Task<IActionResult> GetProfolio(string symbol)
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


        [HttpPost("ask-ai")]
        public async Task<IActionResult> AskAI([FromBody] AIChatRequest req)
        {
            if (string.IsNullOrEmpty(req.Message))
                return BadRequest("訊息內容不能為空");

            // 呼叫真正的 AI 邏輯
            var aiResult = await _aiService.ParseTransactionAsync(req.Message);

            if (aiResult.IsSuccess)
            {
                // 這裡可以連動你的 MarketDataService 執行同步[cite: 1]
                // await _marketDataService.SyncStockPriceAsync(aiResult.Symbol);[cite: 1]
                return Ok(aiResult);
            }

            return BadRequest(aiResult);
        }
    }
}
