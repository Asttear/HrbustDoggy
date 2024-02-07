using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hrbust;
using HrbustDoggy.Maui.Services;

namespace HrbustDoggy.Maui.ViewModels;

public partial class TableViewModel : ObservableObject, IQueryAttributable
{
    private readonly HrbustClient _client;
    private readonly DataHelper _dataHelper;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ActualWeek))]
    private ClassTable? _classTable;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DisplayActualWeekCommand))]
    private int _displayWeek;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    private bool _isRefreshing = false;

    [ObservableProperty]
    private DateTime _now = DateTime.Now;

    public TableViewModel(HrbustClient client, DataHelper dataHelper)
    {
        _client = client;
        _dataHelper = dataHelper;
    }

    public int ActualWeek => ClassTable?.GetWeek(DateOnly.FromDateTime(DateTime.Now)) ?? 0;

    private bool HasClassTable => ClassTable is not null;
    private bool CanRefresh => !IsRefreshing;
    private bool CanDisplayActualWeek => DisplayWeek != ActualWeek;

    [RelayCommand]
    public async Task PrepareDataAsync()
    {
        if (HasClassTable)
        {
            return;
        }
        if (_dataHelper.FileExist())
        {
            try
            {
                LoadFile();
                if (ActualWeek == 0 && ClassTable?.DateWhenObtained != DateOnly.FromDateTime(Now))
                {
                    if (await Shell.Current.DisplayAlert("提示", "课表可能存在变动，是否立即刷新？", "确定", "取消"))
                    {
                        await RefreshAsync();
                    }
                }
                return;
            }
            catch (Exception e)
            {
                await ShowExceptionAsync(e);
            }
        }
        IsRefreshing = true;
    }

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    public async Task RefreshAsync()
    {
        // For compatibility with RefreshView
        if (!IsRefreshing)
        {
            IsRefreshing = true;
            return;
        }
        if (!_client.IsLoggedIn)
        {
            if (await Shell.Current.DisplayAlert("尚未登录", "是否立即登录以刷新课程表？", "确定", "取消"))
            {
                await Shell.Current.GoToAsync("login");
            }
            IsRefreshing = false;
            return;
        }
        try
        {
            ClassTable = await Task.Run(() => _client.GetClassTableAsync());
            DisplayWeek = ActualWeek;
            SaveFile();
            IsRefreshing = false;
            await Shell.Current.DisplayAlert("刷新成功", "课表刷新成功！", "确认");
        }
        catch (Exception e)
        {
            IsRefreshing = false;
            await ShowExceptionAsync(e);
        }
    }

    [RelayCommand(CanExecute = nameof(CanDisplayActualWeek))]
    public void DisplayActualWeek() => DisplayWeek = ActualWeek;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Refresh"] is true)
        {
            IsRefreshing = true;
        }
    }

    private static Task ShowExceptionAsync(Exception e) => Shell.Current.DisplayAlert("出现异常", e.Message, "确认");

    private void LoadFile()
    {
        _dataHelper.Load();
        ClassTable = _dataHelper.ClassTable;
        DisplayWeek = ActualWeek;
    }

    private void SaveFile()
    {
        _dataHelper.ClassTable = ClassTable;
        _dataHelper.Save();
    }
}