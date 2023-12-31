﻿using H.NotifyIcon;
using HrbustDoggy.Wpf.Properties;
using HrbustDoggy.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace HrbustDoggy.Wpf.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Settings _settings;
    private readonly MainViewModel _viewModel;
    private bool _closing = false;

    public MainWindow(Settings settings, MainViewModel viewmodel, TaskbarIcon taskbarIcon)
    {
        InitializeComponent();
        _settings = settings;
        DataContext = _viewModel = viewmodel;
        Panel.Children.Insert(0, taskbarIcon);
    }

    public void Close(bool close)
    {
        _closing = close;
        Close();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.PrepareDataAsync();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (_settings.IsNotificationEnabled && !_closing)
        {
            Hide();
            e.Cancel = true;
            return;
        }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _settings.Save();
    }

    private void Exam_Click(object sender, RoutedEventArgs e)
    {
        ExamsWindow window = App.Current.Services.GetRequiredService<ExamsWindow>();
        window.ShowDialog();
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        string? version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        string message = $"""
            哈尔滨理工大学教务在线客户端
            理工汪 V{version} 内部测试版
            
            ©2023 幺零贰实验室，版权没有。
            
            点击“确定”以访问项目主页。
            """;
        MessageBoxResult result = MessageBox.Show(this, message, "关于", MessageBoxButton.OKCancel, MessageBoxImage.Information);
        if (result == MessageBoxResult.OK)
        {
            Process.Start(new ProcessStartInfo("https://github.com/Asttear/HrbustDoggy") { UseShellExecute = true });
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }
}