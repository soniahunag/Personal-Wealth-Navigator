namespace PWN.Shared
{
    public class AIChatAPIModel
    {

    }
    // 使用者傳給 AI 的文字
    public class AIChatRequest
    {
      //  public string UserId { get; set; } = string.Empty;
        public Guid Reqid { get; set; } = Guid.NewGuid();
        public string Message { get; set; } = string.Empty;
    }
    // AI 解析後的結構化結果
    public class AIParsedResult
    {
        public string Action { get; set; } = string.Empty; // "Buy" or "Sell"
        public string Symbol { get; set; } = string.Empty; // "0050.TW"
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsSuccess { get; set; }
        public string RawAnalysis { get; set; } = string.Empty; // AI 的原始碎碎念
    }   
    //AI 回的內容
    public class AIChatResponse
    {
        public Guid Reqid { get; set; } = Guid.NewGuid();
        public string Reply { get; set; } = string.Empty;
        public AIParsedResult? ParsedResult { get; set; }
    }
}
