using H.NotifyIcon;
using System.Windows;

namespace HrbustDoggy.Wpf.Views;

/// <summary>
/// TrayIcon.xaml 的交互逻辑
/// </summary>
public partial class TrayIcon : TaskbarIcon
{
    public TrayIcon()
    {
        InitializeComponent();
    }

    private void Show_Click(object sender, RoutedEventArgs e)
    {
        App.Current.MainWindow.Show();
        App.Current.MainWindow.Focus();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        ((MainWindow)App.Current.MainWindow).Close(true);
    }
}
