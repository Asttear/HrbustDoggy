using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;
using System.Reflection;

namespace HrbustDoggy.Maui.ViewModels;

public partial class AboutViewModel : ObservableObject
{
    private const string HomePage = "https://github.com/Asttear/HrbustDoggy";

    private readonly IAudioManager _audioManager;
    private readonly Version? _version = Assembly.GetExecutingAssembly().GetName().Version;

    public AboutViewModel(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public string InfoText => $"""
        哈尔滨理工大学教务在线客户端
        理工汪 V{_version} 内部测试版
        
        ©2023 幺零贰实验室，版权没有。
        """;

    [RelayCommand]
    public static async Task OpenHomePageAsync() => await Launcher.OpenAsync(HomePage);

    [RelayCommand]
    public async Task WoofAsync(Page page)
    {
        IAudioPlayer audioPlayer = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("woof.wav"));
        audioPlayer.Play();
        await page.DisplayAlert("汪汪", "汪！汪汪！汪汪汪汪汪汪！", "汪汪！");
        audioPlayer.Stop();
    }
}
