using Hrbust;
using HrbustDoggy.Maui.Services;
using HrbustDoggy.Maui.ViewModels;
using HrbustDoggy.Maui.Views;
using Plugin.Maui.Audio;

namespace HrbustDoggy.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Material-Design-Iconic-Font.ttf", "MDI");
            });

        // Services
        builder.Services.AddSingleton<HrbustClient>();
        builder.Services.AddSingleton<DataHelper>();
        builder.Services.AddSingleton(Preferences.Default);
        builder.Services.AddSingleton(AudioManager.Current);

        // Views
        builder.Services.AddTransient<TablePage>();
        builder.Services.AddTransient<ExamPage>();
        builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<LoginPage>();

        // Viewmodels
        builder.Services.AddTransient<TableViewModel>();
        builder.Services.AddTransient<ExamViewModel>();
        builder.Services.AddTransient<LoginViewModel>();

        AllowMultiLineTruncation();

        return builder.Build();
    }

    private static void AllowMultiLineTruncation()
    {
        // See https://github.com/dotnet/maui/discussions/5492
        static void UpdateMaxLines(Microsoft.Maui.Handlers.LabelHandler handler, ILabel label)
        {
#if ANDROID
            var textView = handler.PlatformView;
            if (label is Label controlsLabel
                && controlsLabel.MaxLines != -1
                && textView.Ellipsize == Android.Text.TextUtils.TruncateAt.End)
            {
                textView.SetMaxLines(controlsLabel.MaxLines);
            }
#endif
        };

        Label.ControlsLabelMapper.AppendToMapping(nameof(Label.LineBreakMode), UpdateMaxLines);
        Label.ControlsLabelMapper.AppendToMapping(nameof(Label.MaxLines), UpdateMaxLines);
    }
}