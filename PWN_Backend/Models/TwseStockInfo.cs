namespace PWN_Backend.Models
{
    public class TwseStockInfo
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        // 證交所回傳的是字串，我們之後再手動 Parse 成 decimal
        public string ClosingPrice { get; set; } = string.Empty;
        public string MonthlyAveragePrice { get; set; } = string.Empty;
    }
}
