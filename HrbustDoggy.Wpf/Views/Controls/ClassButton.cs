using CommunityToolkit.Mvvm.Input;
using Hrbust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace HrbustDoggy.Wpf.Views.Controls;

/// <summary>
/// 课程表按钮控件。
/// </summary>
public class ClassButton : Control
{
    public static readonly DependencyProperty ClassProperty =
        DependencyProperty.Register(nameof(Class), typeof(Class), typeof(ClassButton),
            new PropertyMetadata(null, OnClassChanged));

    public static readonly DependencyProperty WeekProperty =
        DependencyProperty.Register(nameof(Week), typeof(int), typeof(ClassButton),
            new PropertyMetadata(0, OnWeekChanged));

    public static readonly DependencyProperty AccentColorProperty =
        DependencyProperty.Register(nameof(AccentColor), typeof(Brush), typeof(ClassButton),
            new PropertyMetadata(new SolidColorBrush(Colors.Red)));

    public static readonly DependencyProperty ColorMapProperty =
        DependencyProperty.Register(nameof(ColorMap), typeof(Dictionary<string, SolidColorBrush>), typeof(ClassButton),
            new PropertyMetadata(null));

    private readonly RelayCommand _showInfoCommand;
    private readonly ColorAnimation _animation;
    private readonly Storyboard _storyboard;

    static ClassButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ClassButton), new FrameworkPropertyMetadata(typeof(ClassButton)));
    }

    public ClassButton()
    {
        _showInfoCommand = new(ShowClassInfo);
        _animation = new()
        {
            Duration = new Duration(TimeSpan.FromMilliseconds(300))
        };
        Storyboard.SetTargetProperty(_animation, new PropertyPath("Background.Color", null));
        _storyboard = new();
        _storyboard.Children.Add(_animation);
    }

    public Class? Class
    {
        get => GetValue(ClassProperty) as Class;
        set => SetValue(ClassProperty, value);
    }

    public int Week
    {
        get => (int)GetValue(WeekProperty);
        set => SetValue(WeekProperty, value);
    }

    public Brush? AccentColor
    {
        get => GetValue(AccentColorProperty) as Brush;
        set => SetValue(AccentColorProperty, value);
    }

    public Dictionary<string, SolidColorBrush>? ColorMap
    {
        get => GetValue(ColorMapProperty) as Dictionary<string, SolidColorBrush>;
        set => SetValue(ColorMapProperty, value);
    }

    private Button? ButtonPart { get; set; }

    private TextBlock? InfoPart { get; set; }

    private TextBlock? MarkPart { get; set; }

    public override void OnApplyTemplate()
    {
        ButtonPart = GetTemplateChild("PART_Button") as Button;
        InfoPart = GetTemplateChild("PART_Info") as TextBlock;
        MarkPart = GetTemplateChild("PART_Mark") as TextBlock;

        if (ButtonPart is not null)
        {
            ButtonPart.Command = _showInfoCommand;
        }
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        if (Background is not SolidColorBrush brush)
        {
            return;
        }
        Color color = brush.Color + (Color)ColorConverter.ConvertFromString("#7F7F7F");
        _animation.To = color;
        _storyboard.Begin(this);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        _animation.To = null;
        _storyboard.Begin(this);
    }

    private static void OnClassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ClassButton control)
        {
            return;
        }
        if (e.NewValue is not Class aClass || aClass.Count == 0)
        {
            control.Visibility = Visibility.Hidden;
        }
        else
        {
            control.Visibility = Visibility.Visible;
            if (control.MarkPart is not null)
            {
                control.MarkPart.Visibility = aClass.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            control.Update();
        }
    }

    private static void OnWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ClassButton control)
        {
            return;
        }
        control.Update();
    }

    private void Update()
    {
        if (Class is null)
        {
            return;
        }
        List<Course> actives = Class.ScheduledAt(Week);
        BorderThickness = actives.Count > 1 ? new Thickness(3) : new Thickness(0);
        if (actives.Count == 0)
        {
            if (InfoPart is not null)
            {
                InfoPart.Text = "本周无课程";
            }
            Background = ColorMap?["Inactive"].CloneCurrentValue();
        }
        else
        {
            if (InfoPart is not null)
            {
                InfoPart.Text = actives[0].ToString();
            }
            Background = ColorMap?[actives[0].Title].CloneCurrentValue();
        }
    }

    private void ShowClassInfo()
    {
        if (Class is null || Class.Count == 0)
        {
            MessageBox.Show("暂无课程信息。", "课程信息", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        IEnumerable<string> courseInfos = Class.Select(c =>
            $"""
             课程：{c.Title}
             地点：{c.Location}
             教师：{c.Teacher}
             排课：{c.Schedule.RawText}
             其他：{string.Join('\n', c.Description)}
             """);
        MessageBox.Show(string.Join("\n\n", courseInfos), "课程信息", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}