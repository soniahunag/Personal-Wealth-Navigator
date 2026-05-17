using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWN.Shared;
using PWN_Backend.Data;
using PWN_Backend.Models.Entites;
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
        private readonly ApplicationDbContext _dbContext;
        public TransactionController(MarketDataService service,ApplicationDbContext dbContext)
        {
            _marketDataService = service;
            _dbContext = dbContext;

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

        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionAPIModel dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Transaction data is required.");

                if (string.IsNullOrWhiteSpace(dto.TransactionType))
                    return BadRequest("TransactionType is required.");

                if (dto.TransactionType != "Buy" && dto.TransactionType != "Sell")
                    return BadRequest("TransactionType must be Buy or Sell.");

                if (string.IsNullOrWhiteSpace(dto.Symbol))
                    return BadRequest("Symbol is required.");

                if (dto.Quantity <= 0)
                    return BadRequest("Quantity must be greater than zero.");

                if (dto.Price <= 0)
                    return BadRequest("Price must be greater than zero.");

                var now = DateTime.Now;
                var amount = dto.Quantity * dto.Price;

                var transaction = new Transaction
                {
                    Type = dto.TransactionType,
                    Amount = amount,
                    Category = "Stock",
                    TxnDate = DateTime.Now,
                    Note = string.IsNullOrWhiteSpace(dto.Note)
                        ? $"AI parsed stock transaction: {dto.TransactionType} {dto.Quantity} shares of {dto.Symbol} at {dto.Price}"
                        : dto.Note,
                    CreatedAt = now,
                    UpdatedAt = now,
                    UserId = "demo-user",
                    Symbol = dto.Symbol.Trim(),
                    Quantity = dto.Quantity,
                    Price = dto.Price
                };

                _dbContext.Transactions.Add(transaction);
                await _dbContext.SaveChangesAsync();

                await _marketDataService.SyncStockPriceAsync(dto.Symbol);

                return Ok(new
                {
                    Message = "Transaction created successfully.",
                    Data = new
                    {
                        transaction.Id,
                        transaction.Type,
                        transaction.Symbol,
                        transaction.Quantity,
                        transaction.Price,
                        transaction.Amount,
                        transaction.TxnDate
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Create transaction failed.",
                    Error = ex.Message,
                    InnerError = ex.InnerException?.Message
                });
            }
        }
    }
}
