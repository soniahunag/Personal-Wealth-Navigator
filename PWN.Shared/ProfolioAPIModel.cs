namespace PWN.Shared
{
    /// <summary>
    /// 用於回傳特定標的資產概況的資料傳輸物件 (回傳WPF 前端程式 接收計算結果)
    /// </summary>
    public class ProfolioAPIModel
    {
        /// <summary>
        /// 股票代碼 (例如: 0050.TW)
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 目前總持有股數 (買入總數 - 賣出總數)
        /// </summary>
        public decimal CurrentHoldings { get; set; }

        /// <summary>
        /// 資料庫中最新的市場價格
        /// </summary>
        public decimal LatestPrice { get; set; }

        /// <summary>
        /// 目前資產總市值 (持有股數 * 最新市價)
        /// </summary>
        public decimal TotalMarketValue { get; set; }

        /// <summary>
        /// 市場價格最後更新時間 (對應 Yahoo Finance 抓取的日期)[cite: 1]
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
