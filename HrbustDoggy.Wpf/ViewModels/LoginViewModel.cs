using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hrbust;
using HrbustDoggy.Wpf.Properties;
using HrbustDoggy.Wpf.Views;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HrbustDoggy.Wpf.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly HrbustClient _client;
    private readonly Settings _settings;

    [ObservableProperty]
    private string _captchaCode = "";

    [ObservableProperty]
    private BitmapFrame? _captchaImage = null;

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private bool _rememberPassword = false;

    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool _canLogin = true;

    public LoginViewModel(Settings settings, HrbustClient client)
    {
        _settings = settings;
        _client = client;
        // 载入配置
        _captchaCode = "";
        _username = _settings.Username;
        _password = _settings.Password;
        _rememberPassword = _settings.RememberPassword;
        _ = RefreshCaptchaAsync();
    }

    [RelayCommand]
    public async Task RefreshCaptchaAsync()
    {
        try
        {
            CaptchaImage = BitmapFrame.Create(await _client.GetCaptchaAsync());
        }
        catch (HttpRequestException e)
        {
            MessageBox.Show($"出现异常：\n{e.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        CaptchaCode = "";
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    public async Task LoginAsync(LoginWindow loginWindow)
    {
        CanLogin = false;
        try
        {
            switch (await _client.LoginAsync(Username, Password, CaptchaCode))
            {
                case LoginResult.Success:
                    SavePreferences();
                    MessageBox.Show("登录成功！", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    loginWindow.Close();
                    break;

                case LoginResult.CaptchaError:
                    MessageBox.Show("验证码错误！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    await RefreshCaptchaAsync();
                    break;

                case LoginResult.CredentialError:
                    MessageBox.Show("用户名或密码错误！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    await RefreshCaptchaAsync();
                    break;

                default:
                    break;
            }
        }
        catch (HttpRequestException e)
        {
            MessageBox.Show($"出现异常：\n{e.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        CanLogin = true;
    }

    [RelayCommand]
    public void SavePreferences()
    {
        // 保存配置
        _settings.Username = Username;
        if (_settings.RememberPassword = RememberPassword)
        {
            _settings.Password = Password;
        }
        else
        {
            _settings.Password = null;
        }
    }
}