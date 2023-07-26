using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hrbust;
using HrbustDoggy.Wpf.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HrbustDoggy.Wpf.ViewModels;

public partial class ExamViewModel : ObservableObject
{
    private const string DefaultFileName = "Exams.xml";

    private readonly HrbustClient _client;
    private readonly DataHelper _dataHelper;

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
    }

    public IEnumerable<Exam>? CurrentExams => ShowOutdated ? Exams : Exams?.Where(e => e.Time.End > DateTime.Now);

    private bool CanRefresh => !IsRefreshing;

    private Exam[]? Exams
    {
        get => _exams;
        set
        {
            _exams = value;
            OnPropertyChanged(nameof(CurrentExams));
        }
    }

    [RelayCommand]
    public async Task PrepareDataAsync()
    {
        if (Exams is not null)
        {
            return;
        }
        if (File.Exists(DefaultFileName))
        {
            try
            {
                LoadFile(DefaultFileName);
                return;
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }
        await RefreshAsync();
    }

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    public async Task RefreshAsync()
    {
        if (!_client.IsLoggedIn)
        {
            MessageBox.Show("请先登录！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        IsRefreshing = true;
        try
        {
            Exams = await _client.GetExamsAsync();
            SaveFile(DefaultFileName);
            IsRefreshing = false;
            MessageBox.Show("考试信息刷新成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            IsRefreshing = false;
            ShowException(e);
        }
    }

    private static void ShowException(Exception e) => MessageBox.Show($"出现异常：\n{e.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);

    private void LoadFile(string path) => Exams = _dataHelper.LoadExams(path);

    private void SaveFile(string path) => _dataHelper.SaveExams(path, Exams);
}