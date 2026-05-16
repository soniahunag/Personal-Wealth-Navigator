using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PWN.Shared;
using PWN_Backend.Services;

namespace PWN_Backend.Controllers
{
    /// <summary>
    /// AIChatController 負責處理與 AI 相關的 API 請求，特別是解析使用者的自然語言輸入以提取交易資訊。
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AIChatController : ControllerBase
    {

        private readonly AIService _aIService;
        private readonly ILogger<AIChatController> _logger;
        public AIChatController(AIService aIService, ILogger<AIChatController> logger)
        {
            _aIService = aIService;
            _logger = logger;
        }
        [HttpPost("parse-transaction")]
        public async Task<IActionResult> ChatWithAI([FromBody] AIChatRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("AI parse request failed. Request body is null.");

                return BadRequest(new AIChatResponse
                {
                    Reqid = Guid.NewGuid(),
                    Reply = "請提供有效的請求內容。",
                    ParsedResult = new AIParsedResult
                    {
                        IsSuccess = false,
                        RawAnalysis = "Request body is null."
                    }
                });
            }

            if (request.Reqid == Guid.Empty)   //避免空 GUID 進 log。
            {
                request.Reqid = Guid.NewGuid();
            }


            if (string.IsNullOrWhiteSpace(request.Message))
            {
                _logger.LogWarning(
    "AI parse request failed. RequestId: {RequestId}, Reason: {Reason}",
    request.Reqid,
    "請提供有效的訊息");
                return BadRequest(new AIChatResponse
                {
                    Reqid = request.Reqid,
                    Reply = "請提供有效的訊息。",
                    ParsedResult = new AIParsedResult { IsSuccess = false, RawAnalysis = "輸入訊息為空或僅包含空白。" }
                });
            }
            var result = await _aIService.ParseTransactionAsync(request.Message);
            if (result.IsSuccess)
            {
                _logger.LogInformation(
                     "AI parse request success. RequestId: {RequestId}, Action: {Action}, Symbol: {Symbol}, Quantity: {Quantity}, Price: {Price}",
                     request.Reqid,
                     result.Action,
                     result.Symbol,
                     result.Quantity,
                     result.Price
                 );
                return Ok(new AIChatResponse
                {
                    Reqid = request.Reqid,
                    Reply = "解析成功！",
                    ParsedResult = result
                });
            }
            else
            {
                _logger.LogWarning(
   "AI parse request failed. RequestId: {RequestId}, Reason: {Reason}",
   request.Reqid,
   "解析失敗，請輸入正確格式");
                return BadRequest(new AIChatResponse
                {
                    Reqid = request.Reqid,
                    Reply = "解析失敗，請輸入正確格式: 今天買了兩張0050。",
                    ParsedResult = result
                });


            }
        }
    }
}
