using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hrbust;

namespace HrbustDoggy.Maui.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly HrbustClient _client;
    private readonly IPreferences _preferences;

    [ObservableProperty]
    private ImageSource? _captchaImage = null;

    [ObservableProperty]
    private string _captchaCode = "";

    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private bool _rememberPassword = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool _canLogin = true;

    public LoginViewModel(HrbustClient client, IPreferences preferences)
    {
        _client = client;
        _preferences = preferences;
        // 载入配置
        _captchaCode = "";
        _username = _preferences.Get(nameof(Username), "");
        _password = _preferences.Get(nameof(Password), "");
        _rememberPassword = _preferences.Get(nameof(RememberPassword), false);
        RefreshCaptcha();
    }

    [RelayCommand]
    public void RefreshCaptcha()
    {
        var image = ImageSource.FromStream(c => Task.Run(_client.GetCaptchaAsync, c));
        CaptchaImage = image;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    public async Task LoginAsync()
    {
        CanLogin = false;
        try
        {
            if (_client.IsLoggedIn)
            {
                await _client.LogoutAsync();
            }
            switch (await _client.LoginAsync(Username, Password, CaptchaCode))
            {
                case LoginResult.Success:
                    await Shell.Current.GoToAsync("..", new Dictionary<string, object>() { { "Refresh", true } });
                    break;

                case LoginResult.CaptchaError:
                    await Shell.Current.DisplayAlert("登录失败", "验证码错误！", "确定");
                    RefreshCaptcha();
                    break;

                case LoginResult.CredentialError:
                    await Shell.Current.DisplayAlert("登录失败", "用户名或密码错误！", "确定");
                    RefreshCaptcha();
                    break;

                default:
                    break;
            }
        }
        catch (HttpRequestException e)
        {
            await Shell.Current.DisplayAlert("出现异常", e.Message, "确定");
        }
        CanLogin = true;
    }

    [RelayCommand]
    public void SavePreferences()
    {
        // 保存配置
        _preferences.Set(nameof(Username), Username);
        _preferences.Set(nameof(RememberPassword), RememberPassword);
        if (RememberPassword)
        {
            _preferences.Set(nameof(Password), Password);
        }
        else
        {
            _preferences.Set(nameof(Password), "");
        }
    }
}