using PWN.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PWN_Simulator.Services
{
    public class APIServices
    {
        private readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("https://localhost:7166/") }; // 記得換成你 Backend 的實際 Port

        // 呼叫資產概況 API
        public async Task<ProfolioAPIModel?> GetPortfolioAsync(string symbol)
        {
            return await _client.GetFromJsonAsync<ProfolioAPIModel>($"/api/Transaction/portfolio/{symbol}");
        }

        // 呼叫建立交易 API
        public async Task<bool> CreateTransactionAsync(TransactionAPIModel dto)
        {
            var response = await _client.PostAsJsonAsync("api/Transaction/create", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<AIChatResponse?> ParseTransactionWithAIAsync(string message)
        {
            var request = new AIChatRequest
            {
                Reqid = Guid.NewGuid(),
                Message = message
            };
            var response = await _client.PostAsJsonAsync("api/AIChat/parse-transaction", request);
            return await response.Content.ReadFromJsonAsync<AIChatResponse>();

        }
    }
}
