namespace PWN.Shared
{
    /// <summary>
    /// 用於建立接收WPF給的新交易紀錄的資料傳輸物件
    /// </summary>
    public class TransactionAPIModel
    {
        /// <summary>
        /// 股票代碼 (例如: 0050.TW)
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// 成交股數 (例如: 1000)
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 成交單價 (例如: 150.50)
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 交易日期與時間
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// 交易類型: Buy (買入) 或 Sell (賣出)
        /// </summary>
        public string TransactionType { get; set; } = "Buy";
    }
}
