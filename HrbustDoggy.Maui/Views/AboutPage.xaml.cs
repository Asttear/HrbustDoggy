using HrbustDoggy.Maui.ViewModels;

namespace HrbustDoggy.Maui.Views;

public partial class AboutPage : ContentPage
{
    public AboutPage(AboutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}