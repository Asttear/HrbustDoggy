using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hrbust;
using HrbustDoggy.Wpf.Properties;
using HrbustDoggy.Wpf.Services;
using HrbustDoggy.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace HrbustDoggy.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const string DefaultFileName = "ClassData.xml";

    private readonly HrbustClient _client;
    private readonly DataHelper _dataHelper;
    private readonly ClassNotification _notification;
    private readonly Settings _settings;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExportCommand))]
    [NotifyPropertyChangedFor(nameof(ActualWeek))]
    private ClassTable? _classTable;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DisplayActualWeekCommand))]
    private int _displayWeek;

    [ObservableProperty]
    private string _status = "未载入课表";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    [NotifyCanExecuteChangedFor(nameof(LogoutCommand))]
    private bool _isLoggedIn = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    private bool _isRefreshing = false;

    [ObservableProperty]
    private DateTime _now = DateTime.Now;

    public MainViewModel(HrbustClient client, DataHelper dataHelper, ClassNotification notification, Settings settings, Clock clock)
    {
        _client = client;
        _dataHelper = dataHelper;
        _notification = notification;
        _settings = settings;
        notification.IsEnabled = settings.IsNotificationEnabled;
        notification.TimeAdvance = settings.NotificationTimeAdvance;
        clock.OnNextDay += Clock_OnNextDay;
    }

    public int ActualWeek => ClassTable?.GetWeek(DateOnly.FromDateTime(DateTime.Now)) ?? 0;

    public bool IsNotificationEnabled
    {
        get => _notification.IsEnabled;
        set
        {
            _notification.IsEnabled = value;
            _settings.IsNotificationEnabled = value;
            OnPropertyChanged();
        }
    }

    public int NotificationTimeAdvance
    {
        get => _notification.TimeAdvance;
        set
        {
            _notification.TimeAdvance = value;
            _settings.NotificationTimeAdvance = value;
            OnPropertyChanged();
        }
    }

    private bool HasClassTable => ClassTable is not null;
    private bool CanLogin => !IsLoggedIn;
    private bool CanRefresh => !IsRefreshing;
    private bool CanDisplayActualWeek => DisplayWeek != ActualWeek;

    [RelayCommand]
    public async Task PrepareDataAsync()
    {
        if (HasClassTable)
        {
            return;
        }
        if (File.Exists(DefaultFileName))
        {
            try
            {
                LoadFile(DefaultFileName);
                if (ActualWeek == 0 && ClassTable?.DateWhenObtained != DateOnly.FromDateTime(Now))
                {
                    MessageBoxResult result = MessageBox.Show("课表可能存在变动，是否立即刷新？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.OK)
                    {
                        await RefreshAsync();
                    }
                }
                return;
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }
        await RefreshAsync();
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    public void Login()
    {
        App.Current.Services.GetRequiredService<LoginWindow>().ShowDialog();
        IsLoggedIn = _client.IsLoggedIn;
    }

    [RelayCommand(CanExecute = nameof(IsLoggedIn))]
    public async Task LogoutAsync()
    {
        try
        {
            await _client.LogoutAsync();
            IsLoggedIn = _client.IsLoggedIn;
            MessageBox.Show("注销成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            ShowException(e);
        }
    }

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    public async Task RefreshAsync()
    {
        if (!IsLoggedIn)
        {
            if (MessageBox.Show("尚未登录，是否立即登录以刷新课程表？",
                                "消息",
                                MessageBoxButton.OKCancel,
                                MessageBoxImage.Question,
                                MessageBoxResult.OK) != MessageBoxResult.OK)
            {
                return;
            }
            Login();
            if (!IsLoggedIn)
            {
                return;
            }
        }
        IsRefreshing = true;
        try
        {
            Status = "正在刷新课表……";
            ClassTable = await _client.GetClassTableAsync();
            DisplayWeek = ActualWeek;
            Status = ActualWeek > 0 ? $"第{ActualWeek}周" : "未开学";
            _notification.ClassTable = ClassTable;
            SaveFile(DefaultFileName);
            IsRefreshing = false;
            MessageBox.Show("课表刷新成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            Status = "课表刷新失败！";
            IsRefreshing = false;
            ShowException(e);
        }
    }

    [RelayCommand]
    public void Import()
    {
        OpenFileDialog dialog = new()
        {
            FileName = DefaultFileName,
            DefaultExt = "xml",
            InitialDirectory = Environment.CurrentDirectory,
            Filter = "课程表数据文件|*.xml"
        };
        if (dialog.ShowDialog() != true)
        {
            return;
        }
        try
        {
            LoadFile(dialog.FileName);
        }
        catch (Exception e)
        {
            ShowException(e);
        }
    }

    [RelayCommand(CanExecute = nameof(HasClassTable))]
    public void Export()
    {
        SaveFileDialog dialog = new()
        {
            FileName = DefaultFileName,
            DefaultExt = "xml",
            InitialDirectory = Environment.CurrentDirectory,
            Filter = "课程表数据文件|*.xml"
        };
        if (dialog.ShowDialog() != true)
        {
            return;
        }
        try
        {
            SaveFile(dialog.FileName);
            MessageBox.Show("保存文件成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            ShowException(e);
        }
    }

    [RelayCommand(CanExecute = nameof(CanDisplayActualWeek))]
    public void DisplayActualWeek() => DisplayWeek = ActualWeek;

    private static void ShowException(Exception e) => MessageBox.Show($"出现异常：\n{e.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);

    private void Clock_OnNextDay(object? sender, System.Timers.ElapsedEventArgs e)
    {
        Now = e.SignalTime;
        OnPropertyChanged(nameof(ActualWeek));
    }

    private void LoadFile(string path)
    {
        ClassTable = _dataHelper.LoadClassTable(path);
        DisplayWeek = ActualWeek;
        Status = ActualWeek > 0 ? $"第{ActualWeek}周" : "未开学";
        _notification.ClassTable = ClassTable;
    }

    private void SaveFile(string path)
        => _dataHelper.SaveClassTable(path, ClassTable);
}