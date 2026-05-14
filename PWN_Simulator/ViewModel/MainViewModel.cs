using PWN.Shared;
using PWN_Simulator.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PWN_Simulator.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        //Implement INotifyPropertyChanged for data binding
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly APIServices _aPIServices = new APIServices();
        private ProfolioAPIModel? _portfolio;
        
        //顯示在畫面上的資產資料
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
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
