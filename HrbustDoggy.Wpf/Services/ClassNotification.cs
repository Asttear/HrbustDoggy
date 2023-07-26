using H.NotifyIcon;
using H.NotifyIcon.Core;
using Hrbust;
using HrbustDoggy.Wpf.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Timers;
using System.Windows;

namespace HrbustDoggy.Wpf.Services;

public class ClassNotification
{
    private readonly Clock _clock;
    private readonly TaskbarIcon _notifyIcon;
    private readonly SoundPlayer _soundPlayer = new(Resources.Woof);
    private readonly List<(Class Class, TimeOnly StartTime)> _schedule = new();

    private ClassTable? _classTable;
    private bool _isEnabled = false;

    public ClassNotification(TaskbarIcon notifyIcon, Clock clock)
    {
        _notifyIcon = notifyIcon;
        _clock = clock;
        _notifyIcon.TrayBalloonTipClicked += OnTrayBalloonTipClicked;
        _notifyIcon.TrayMouseDoubleClick += OnTrayMouseDoubleClick;
    }

    public int TimeAdvance { get; set; } = 15;

    public ClassTable? ClassTable
    {
        get => _classTable;
        set
        {
            _classTable = value;
            _schedule.Clear();
            if (value is null)
            {
                return;
            }
            Class[]? classes = value.GetClasses(DateOnly.FromDateTime(DateTime.Now));
            if (classes is null)
            {
                return;
            }
            _schedule.AddRange(classes.Zip(value.StartTimes));
        }
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value)
            {
                _clock.OnNextMinute += OnNextMinute;
                _clock.OnNextDay += OnNextDay;
            }
            else
            {
                _clock.OnNextMinute -= OnNextMinute;
                _clock.OnNextDay -= OnNextDay;
            }
            _isEnabled = value;
        }
    }

    private void OnTrayBalloonTipClicked(object sender, RoutedEventArgs e) => _soundPlayer.Stop();

    private void OnTrayMouseDoubleClick(object sender, RoutedEventArgs e) => _soundPlayer.Stop();

    private void OnNextMinute(object? sender, ElapsedEventArgs e) => Check(e.SignalTime);

    private void OnNextDay(object? sender, ElapsedEventArgs e) => Update(e.SignalTime);

    private void Check(DateTime dateTime)
    {
        TimeOnly now = new(dateTime.Hour, dateTime.Minute);

        foreach (var pair in _schedule)
        {
            (Class aClass, TimeOnly scheduledTime) = pair;
            if (aClass.Count == 0)
            {
                continue;
            }
            TimeOnly expectTime = now.AddMinutes(TimeAdvance);
            if (expectTime == scheduledTime)
            {
                Notify(aClass);
                break;
            }
        }
    }

    private void Notify(Class aClass)
    {
        foreach (Course course in aClass)
        {
            _notifyIcon.ShowNotification(course.Title, $"{course.Location}/{course.Teacher}", NotificationIcon.Info);
        }
        _soundPlayer.Play();
    }

    private void Update(DateTime dateTime)
    {
        _schedule.Clear();
        if (_classTable is null)
        {
            return;
        }
        Class[]? classes = _classTable.GetClasses(DateOnly.FromDateTime(dateTime));
        if (classes is null)
        {
            return;
        }
        _schedule.AddRange(classes.Zip(_classTable.StartTimes));
    }
}