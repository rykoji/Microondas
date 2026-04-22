using System.Windows;
using System.Windows.Controls;
using Microondas.WPF.ViewModels;

namespace Microondas.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MicroondasViewModel();
    }

    private void BotaoLogin_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MicroondasViewModel vm)
            vm.ExecutarLogin(SenhaBox.Password);
    }

    private void AnimacaoScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (e.ExtentHeightChange > 0 && sender is ScrollViewer sv)
            sv.ScrollToEnd();
    }
}