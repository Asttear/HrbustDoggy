using HrbustDoggy.Wpf.ViewModels;
using System.Windows;

namespace HrbustDoggy.Wpf.Views;

/// <summary>
/// ExamsWindow.xaml 的交互逻辑
/// </summary>
public partial class ExamsWindow : Window
{
    private readonly ExamViewModel _examViewModel;

    public ExamsWindow(ExamViewModel examViewModel)
    {
        InitializeComponent();
        DataContext = _examViewModel = examViewModel;
        Owner ??= App.Current.MainWindow;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await _examViewModel.PrepareDataAsync();
    }
}