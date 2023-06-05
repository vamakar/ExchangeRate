using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExchangeRate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            this.DataContext = _viewModel;
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.OnLoadButtonClick();
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.OnSearchButtonClick();
        }

        private void SwapButton_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.OnSwapButtonClick();
        }

        private void ToConvert_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.RefreshConvert();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                _viewModel.OnSearchButtonClick();
            }
        }
    }
}
