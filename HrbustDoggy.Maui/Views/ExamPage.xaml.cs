using HrbustDoggy.Maui.ViewModels;

namespace HrbustDoggy.Maui.Views;

public partial class ExamPage : ContentPage
{
    private readonly ExamViewModel _viewModel;

    public ExamPage(ExamViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await _viewModel.PrepareDataAsync();
    }
}