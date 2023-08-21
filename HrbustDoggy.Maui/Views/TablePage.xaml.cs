using HrbustDoggy.Maui.ViewModels;

namespace HrbustDoggy.Maui.Views;

public partial class TablePage : ContentPage
{
    private readonly TableViewModel _viewModel;

    public TablePage(TableViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await _viewModel.PrepareDataAsync();
    }

    private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
    {
        if (_viewModel.ActualWeek == 0)
        {
            return;
        }
        switch (e.Direction)
        {
            case SwipeDirection.Right:
                if (_viewModel.DisplayWeek > 1)
                {
                    _viewModel.DisplayWeek--;
                }
                break;
            case SwipeDirection.Left:
                if (_viewModel.DisplayWeek < 25)
                {
                    _viewModel.DisplayWeek++;
                }
                break;
            default:
                break;
        }
    }
}