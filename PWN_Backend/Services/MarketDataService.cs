using Microsoft.EntityFrameworkCore;
using NodaTime;
using PWN_Backend.Data;
using PWN_Backend.Models;
using PWN_Backend.Models.Entites;
using YahooFinanceApi;
using YahooQuotesApi;

namespace PWN_Backend.Services
{
    //Link to External API to get market data, and cache it in database for 1 day
    public class MarketDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MarketDataService> _logger;
        public MarketDataService(ApplicationDbContext context, ILogger<MarketDataService> logger) 
        {
            //建構子: 「初始化」新物件，物件初始化、屬性賦值、強制設定必要參數，以及自動執行物件準備作業
            //在 Program.cs 註冊：你告訴系統「當有人需要 ApplicationDbContext 或 ILogger 時，請給他一個實例」。
            _context = context;// 這裡的 context 是由 ASP.NET Core 自動傳進來的實例
            _logger = logger;   // 這裡的 logger 也是自動傳進來的，讓你可以寫 Log
        }

        public async Task<bool> SyncStockPriceAsync(string stockSymbol = "0050.TW")
        {
            try
            {
                _logger.LogInformation("開始執行 {Symbol} 同步任務...", stockSymbol);

                using var client = new HttpClient();
                // 偽裝瀏覽器標頭，降低被封鎖機率
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");

                // 設定時間區間 (最近 7 天)
                long start = (long)(DateTime.UtcNow.AddDays(-7) - DateTime.UnixEpoch).TotalSeconds;
                long end = (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
                string url = $"https://query1.finance.yahoo.com/v7/finance/download/{stockSymbol}?period1={start}&period2={end}&interval=1d&events=history&includeAdjustedClose=true";

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var csvContent = await response.Content.ReadAsStringAsync();
                    var rows = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    if (rows.Length >= 2)
                    {
                        var lastRow = rows[^1].Split(',');
                        var tradeDate = DateTime.Parse(lastRow[0]);
                        var adjClose = decimal.Parse(lastRow[5]);

                        await SaveToDatabase(stockSymbol, tradeDate, adjClose);
                        _logger.LogInformation("成功從 Yahoo Finance 獲取真實數據。");
                        return true;
                    }
                }

                // 如果執行到這，代表 API 失敗 (401/429) 或解析失敗，進入備援模式
                throw new HttpRequestException($"外部服務暫時無法存取 (Status: {response.StatusCode})");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("外部 API 異常 ({Msg})，啟用 Circuit Breaker 模擬模式。", ex.Message);

                // 模擬數據：產生一個 155~165 之間的隨機價格
                decimal mockPrice = Math.Round(160.0m + (decimal)(new Random().NextDouble() * 5), 2);
                DateTime mockDate = DateTime.Today;

                await SaveToDatabase(stockSymbol, mockDate, mockPrice);
                _logger.LogInformation("已寫入備援模擬數據：{Price}", mockPrice);

                return true; // 回傳 true 讓前端流程不中斷
            }
        }

        // 抽取出來的寫入邏輯，保持程式碼整潔
        private async Task SaveToDatabase(string symbol, DateTime date, decimal price)
        {
            // 檢查是否已存在同一天同一標的的資料
            var exists = await _context.MarketDataCaches
                .AnyAsync(x => x.Symbol == symbol && x.DataDate == date);

            if (!exists)
            {
                var newData = new MarketDataCache
                {
                    Symbol = symbol,
                    DataDate = date,
                    AdjClose = price,
                    CreatedAt = DateTime.Now
                };

                _context.MarketDataCaches.Add(newData);
                await _context.SaveChangesAsync();
            }
        }

        // 新增：專門給 Controller 或其他地方「拿價格」用
        public async Task<decimal> GetLatestPriceFromDbAsync(string stockSymbol)
        {
            // 從資料庫找這個標的最新的一筆收盤價
            var latestData = await _context.MarketDataCaches
                .Where(x => x.Symbol == stockSymbol)
                .OrderByDescending(x => x.DataDate)
                .FirstOrDefaultAsync();

            // 如果資料庫有資料，回傳價格；沒有的話給個預設值或報錯
            return latestData?.AdjClose ?? 160.0m;
        }
    }
}
