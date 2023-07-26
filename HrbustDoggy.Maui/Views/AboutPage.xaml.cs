using Plugin.Maui.Audio;
using System.Reflection;

namespace HrbustDoggy.Maui.Views;

public partial class AboutPage : ContentPage
{
    private readonly IAudioManager _audioManager;

    public AboutPage(IAudioManager audioManager)
    {
        InitializeComponent();
        _audioManager = audioManager;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        string? version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        info.Text = $"哈尔滨理工大学教务在线客户端\n理工汪 V{version} 内部测试版\n\n©2023 幺零贰实验室，版权没有。";
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        IAudioPlayer audioPlayer = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("woof.wav"));
        audioPlayer.Play();
        await DisplayAlert("汪汪", "汪！汪汪！汪汪汪汪汪汪！", "汪汪！");
        audioPlayer.Stop();
    }
}