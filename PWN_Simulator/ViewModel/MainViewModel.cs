using PWN.Shared;
using PWN_Simulator.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PWN_Simulator.Helpers;
using System.Windows.Input;

namespace PWN_Simulator.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //Implement INotifyPropertyChanged for data binding
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly APIServices _aPIServices = new APIServices();
        private ProfolioAPIModel? _portfolio;   //用於綁定顯示在畫面上的資產資料

        public MainViewModel()
        {
            AskAICommand = new AsyncRelayCommand(AskAIAsync);
            ConfirmTransactionCommand = new AsyncRelayCommand(
                ConfirmTransactionAsync,
                () => CanConfirmTransaction
            );
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    
        #region 顯示在畫面上的資產資料
        public ProfolioAPIModel? Portfolio
        {
            get => _portfolio;
            set
            {
                _portfolio = value;
                OnPropertyChanged(nameof(Portfolio));
            }
        }
        // 按鈕時觸發連結的function
        public async Task LoadPortfolioAsync(string symbol)
        {
            Portfolio = await _aPIServices.GetPortfolioAsync(symbol);
        }
        #endregion

        #region  顯示 AI 或系統回覆訊息
        private string _aiInputMessage = string.Empty;
        private string _aiReply = string.Empty;

        // 使用者在 WPF 輸入的自然語言，例如：今天買入 2 張 0050，價格 150
        public string AIInputMessage
        {
            get => _aiInputMessage;
            set
            {
                _aiInputMessage = value;
                OnPropertyChanged();
            }
        }
        
        public string AIReply
        {
            get => _aiReply;
            set
            {
                _aiReply = value;
                OnPropertyChanged();
            }
        }

        private AIParsedResult? _parsedResult;

        // AI 解析後的交易結果
        public AIParsedResult? ParsedResult
        {
            get => _parsedResult;
            set
            {
                _parsedResult = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanConfirmTransaction));

                if (ConfirmTransactionCommand is AsyncRelayCommand command)
                {
                    command.RaiseCanExecuteChanged();
                }
            }
        }

        // 控制 Confirm Save 按鈕是否可以按
        public bool CanConfirmTransaction =>
            ParsedResult != null && ParsedResult.IsSuccess;

        public ICommand AskAICommand { get; }
        public ICommand ConfirmTransactionCommand { get; }
        // AI 解析使用者輸入
        private async Task AskAIAsync()
        {
            if (string.IsNullOrWhiteSpace(AIInputMessage))
            {
                AIReply = "請輸入交易內容，例如：今天買入 2 張 0050，價格 150。";
                ParsedResult = null;
                return;
            }

            try
            {
                AIReply = "AI 解析中...";

                var response = await _aPIServices.ParseTransactionWithAIAsync(AIInputMessage);

                if (response?.ParsedResult?.IsSuccess == true)
                {
                    ParsedResult = response.ParsedResult;
                    AIReply = response.Reply;
                }
                else
                {
                    ParsedResult = null;
                    AIReply = response?.Reply ?? "AI 無法解析這筆交易。";
                }
            }
            catch (Exception ex)
            {
                ParsedResult = null;
                AIReply = $"呼叫 AI 服務失敗：{ex.Message}";
            }
        }

        // 使用者確認後，將 AI 解析結果寫入 DB
        private async Task ConfirmTransactionAsync()
        {
            if (ParsedResult == null || !ParsedResult.IsSuccess)
            {
                AIReply = "目前沒有可儲存的交易資料。";
                return;
            }

            try
            {
                AIReply = "交易儲存中...";

                var dto = new TransactionAPIModel
                {
                    TransactionType = ParsedResult.Action,
                    Symbol = ParsedResult.Symbol,
                    Quantity = ParsedResult.Quantity,
                    Price = ParsedResult.Price
                };

                var success = await _aPIServices.CreateTransactionAsync(dto);

                if (success)
                {
                    AIReply = "交易已成功儲存。";
                    ParsedResult = null;
                    AIInputMessage = string.Empty;
                }
                else
                {
                    AIReply = "交易儲存失敗，請確認後端 API 或資料格式。";
                }
            }
            catch (Exception ex)
            {
                AIReply = $"儲存交易失敗：{ex.Message}";
            }
        }
        #endregion

    }
}
