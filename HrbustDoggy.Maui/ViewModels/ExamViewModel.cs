using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hrbust;
using HrbustDoggy.Maui.Services;

namespace HrbustDoggy.Maui.ViewModels;

public partial class ExamViewModel : ObservableObject, IQueryAttributable
{
    private const string DefaultFileName = "Exams.xml";

    private readonly HrbustClient _client;
    private readonly DataHelper _dataHelper;
    private readonly string _defaultPath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentExams))]
    private bool _showOutdated;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    private bool _isRefreshing;

    private Exam[]? _exams;

    public ExamViewModel(HrbustClient client, DataHelper dataHelper)
    {
        _client = client;
        _dataHelper = dataHelper;
        _defaultPath = Path.Combine(FileSystem.CacheDirectory, DefaultFileName);
    }

    public IEnumerable<Exam>? CurrentExams => ShowOutdated ? Exams : Exams?.Where(e => e.Time.End > DateTime.Now);

    private Exam[]? Exams
    {
        get => _exams;
        set
        {
            _exams = value;
            OnPropertyChanged(nameof(CurrentExams));
        }
    }

    private bool CanRefresh => !IsRefreshing;

    [RelayCommand]
    public void ToggleShowOutdated() => ShowOutdated = !ShowOutdated;

    [RelayCommand]
    public async Task PrepareDataAsync()
    {
        if (Exams is not null)
        {
            return;
        }
        if (File.Exists(_defaultPath))
        {
            try
            {
                LoadFile(_defaultPath);
                return;
            }
            catch (Exception e)
            {
                await ShowExceptionAsync(e);
            }
        }
        await RefreshAsync();
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
            if (await Shell.Current.DisplayAlert("尚未登录", "是否立即登录以刷新考试信息？", "确定", "取消"))
            {
                await Shell.Current.GoToAsync("login");
            }
            IsRefreshing = false;
            return;
        }
        try
        {
            Exams = await Task.Run(_client.GetExamsAsync);
            SaveFile(_defaultPath);
            IsRefreshing = false;
            await Shell.Current.DisplayAlert("消息", "考试信息刷新成功！", "确定");
        }
        catch (Exception e)
        {
            IsRefreshing = false;
            await ShowExceptionAsync(e);
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Refresh"] is true)
        {
            IsRefreshing = true;
        }
    }

    private static async Task ShowExceptionAsync(Exception e) => await Shell.Current.DisplayAlert("错误", $"出现异常：\n{e.Message}", "确定");

    private void LoadFile(string path) => Exams = _dataHelper.LoadExams(path);

    private void SaveFile(string path) => _dataHelper.SaveExams(path, Exams);
}