using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using PWN.Shared;
using System.Text.Json;
namespace PWN_Backend.Services
{
    /// <summary>
    /// AIService 負責處理所有與 AI 模型（Semantic Kernel）相關的邏輯。
    /// 這是一個單例或暫時性服務，會被注入到 Controller 中使用。
    /// </summary>
    public class AIService
    {
        //for Prompt engineering 
        // Kernel 是 Semantic Kernel 的核心對象，它管理配置、連接器（如 OpenAI）和插件。
        private readonly Kernel _kernel;
        // ILogger 是 ASP.NET Core 內建的日誌介面。
        // 用途：記錄程式執行過程中的資訊、警告或錯誤，方便生產環境除錯。
        private readonly ILogger<AIService> _logger;
        /// <summary>
        /// 建構子注入 (Constructor Injection)
        /// </summary>
        /// <param name="config">IConfiguration：用於讀取 appsettings.json 中的設定值（如 API Key）。</param>
        /// <param name="logger">ILogger：系統自動傳入，用於寫日誌。</param>
        public AIService(IConfiguration config, ILogger<AIService> logger)
        {
            _logger = logger;

            var builder = Kernel.CreateBuilder();   // 初始化 kernal

            //配置AI連接器
            // 【關鍵修改】：切換到 OpenAI 連接器
            // config["OpenAI:ModelId"] 會讀取 appsettings.json 裡的 "gpt-4o"
            builder.AddOpenAIChatCompletion(
                modelId: config["OpenAI:ModelId"] ?? "gpt-4o",
                apiKey: config["OpenAI:ApiKey"] ?? ""
            );

            _kernel = builder.Build(); // 建立 Kernel 實例

        }


        /// <summary>
        /// 將使用者的自然語言訊息傳送給 AI，並要求回傳結構化 JSON。
        /// </summary>
        /// <param name="userMessage">例如：「今天買了 2 張台積電，價格 700」</param>
        public async Task<AIParsedResult> ParseTransactionAsync(string userMessage)
        {
            // System Prompt 定義了 AI 的「角色」與「行為邊界」。
            // 我們使用「少樣本學習 (Few-Shot)」技術提供範例，增加 AI 的穩定性。
            string prompt = @"
            你是一個專業的股票記帳助理。
            請解析使用者的文字，並嚴格回傳一個 JSON 格式的結果。
            
            規則：
            1. Action 必須是 'Buy' 或 'Sell'。
            2. Symbol 必須是 Yahoo Finance 格式 (例如 0050.TW)。
            3. Quantity 必須是整數 (張數請乘以 1000)。
            4. Price 必須是數字。

            範例輸入：昨天買入 2 張 0050 價格 150
            範例回傳：
            {
                ""Action"": ""Buy"",
                ""Symbol"": ""0050.TW"",
                ""Quantity"": 2000,
                ""Price"": 150.0,
                ""IsSuccess"": true
            }

            使用者輸入：{{$input}}";

            try
            {
                // InvokePromptAsync：這是 Semantic Kernel 最常用的方法。
                // 它將 Prompt 發送給雲端模型，並將 {{$input}} 替換為 userMessage。
                var result = await _kernel.InvokePromptAsync(prompt, new KernelArguments { ["input"] = userMessage });

                // result.ToString() 會拿到 AI 回傳的文字內容（預期是 JSON）。
                var jsonResult = result.ToString();

                // JsonSerializer：將字串轉回 C# 物件。
                // PropertyNameCaseInsensitive = true：忽略大小寫差異（防止 AI 回傳 "action" 而不是 "Action"）。
                var parseResult = JsonSerializer.Deserialize<AIParsedResult>(jsonResult, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return parseResult ?? new AIParsedResult { IsSuccess = false };
            }
            catch (Exception ex)
            {
                // 當 API 連線失敗或 JSON 解析出錯時，記錄錯誤訊息。
                _logger.LogError("AI 解析失敗: {Msg}", ex.Message);

                // 回傳一個失敗標記的物件，讓前端知道出事了，而不是讓整個 App 當掉。
                return new AIParsedResult { IsSuccess = false, RawAnalysis = "系統暫時無法解析您的指令。" };
            }
        }
    }
}
