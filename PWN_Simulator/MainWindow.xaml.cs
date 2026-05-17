using PWN_Simulator.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PWN_Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            string symbol = TxtSymbol.Text;
            // 呼叫 ViewModel 的方法去抓資料
            await ViewModel.LoadPortfolioAsync(symbol);
        }
    }
}