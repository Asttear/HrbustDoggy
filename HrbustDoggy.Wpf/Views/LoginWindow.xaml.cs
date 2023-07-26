using HrbustDoggy.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace HrbustDoggy.Wpf.Views;

/// <summary>
/// LoginWindow.xaml 的交互逻辑
/// </summary>
public partial class LoginWindow : Window
{
    private readonly LoginViewModel _model;

    public LoginWindow(LoginViewModel model)
    {
        InitializeComponent();
        DataContext = _model = model;
        Owner ??= App.Current.MainWindow;
    }

    private void PasswordBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
            passwordBox.Password = _model.Password;
    }

    private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
            _model.Password = passwordBox.Password;
    }
}