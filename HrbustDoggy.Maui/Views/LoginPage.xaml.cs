using HrbustDoggy.Maui.ViewModels;

namespace HrbustDoggy.Maui.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnDisappearing()
    {
        _viewModel.SavePreferences();
    }
}