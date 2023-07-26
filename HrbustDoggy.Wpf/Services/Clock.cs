using System;
using System.Timers;

namespace HrbustDoggy.Wpf.Services;

public class Clock
{
    private readonly Timer _timer = new()
    {
        Interval = 1000,
        AutoReset = true,
        Enabled = true
    };

    private DateTime _dateTime;

    public Clock()
    {
        _dateTime = DateTime.Now.Date;
        _timer.Elapsed += Timer_Elapsed;
    }

    public event ElapsedEventHandler? OnNextMinute;

    public event ElapsedEventHandler? OnNextHour;

    public event ElapsedEventHandler? OnNextDay;

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        DateTime old = _dateTime;
        _dateTime = e.SignalTime;
        if (e.SignalTime.Minute != old.Minute)
        {
            OnNextMinute?.Invoke(this, e);
        }
        if (e.SignalTime.Hour != old.Hour)
        {
            OnNextHour?.Invoke(this, e);
        }
        if (e.SignalTime.Day != old.Day)
        {
            OnNextDay?.Invoke(this, e);
        }
    }
}