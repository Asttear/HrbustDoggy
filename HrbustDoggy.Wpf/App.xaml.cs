using CommunityToolkit.Mvvm.DependencyInjection;
using H.NotifyIcon;
using Hrbust;
using HrbustDoggy.Wpf.Properties;
using HrbustDoggy.Wpf.Services;
using HrbustDoggy.Wpf.ViewModels;
using HrbustDoggy.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace HrbustDoggy.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            Services = ConfigureServices();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use.
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Process[] processes = Process.GetProcessesByName(ResourceAssembly.GetName().Name);
            if (processes.Length > 1)
            {
                MessageBox.Show("汪汪已在运行！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(0);
            }
            MainWindow main = Services.GetRequiredService<MainWindow>();
            main.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Services.GetRequiredService<TaskbarIcon>().Dispose();
            base.OnExit(e);
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton(Settings.Default);
            services.AddSingleton<HrbustClient>();
            services.AddSingleton<DataHelper>();
            services.AddSingleton<ClassNotification>();
            services.AddSingleton<Clock>();

            // Viewmodels
            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<ExamViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddSingleton<TaskbarIcon, TrayIcon>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<ExamsWindow>();

            return services.BuildServiceProvider();
        }
    }
}