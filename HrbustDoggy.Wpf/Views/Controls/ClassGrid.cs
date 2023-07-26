using Hrbust;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace HrbustDoggy.Wpf.Views.Controls;

/// <summary>
/// 课程表控件。
/// </summary>
public class ClassGrid : Control
{
    public static readonly DependencyProperty DateNowProperty =
        DependencyProperty.Register(nameof(DateNow), typeof(DateTime), typeof(ClassGrid),
            new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnDateNowChanged)));

    public static readonly DependencyProperty ActualWeekProperty =
        DependencyProperty.Register(nameof(ActualWeek), typeof(int), typeof(ClassGrid),
            new PropertyMetadata(0, new PropertyChangedCallback(OnActualWeekChanged)));

    public static readonly DependencyProperty DisplayWeekProperty =
        DependencyProperty.Register(nameof(DisplayWeek), typeof(int), typeof(ClassGrid),
            new PropertyMetadata(0, new PropertyChangedCallback(OnDisplayWeekChanged)));

    public static readonly DependencyProperty ClassTableProperty =
        DependencyProperty.Register(nameof(ClassTable), typeof(ClassTable), typeof(ClassGrid),
            new PropertyMetadata(null, new PropertyChangedCallback(OnClassTableChanged)));

    private static readonly string[] s_hexColors =
    {
        "#B71C1C", "#D81B60", "#8E24AA", "#5E35B1",
        "#3949AB", "#1E88E5", "#29B6F6", "#00ACC1",
        "#00897B", "#43A047", "#7CB342", "#C0CA33",
        "#FBC02D", "#E65100", "#F4511E", "#6D4C41"
    };

    private static readonly List<SolidColorBrush> s_brushes =
        s_hexColors.Select(s => new SolidColorBrush((Color)ColorConverter.ConvertFromString(s))).ToList();

    private static readonly SolidColorBrush s_inactiveBrush = new(Colors.Gray);

    private readonly Dictionary<string, SolidColorBrush> _colorMap = new();
    private readonly ClassButton[,] _buttons = new ClassButton[7, 6];

    static ClassGrid()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ClassGrid), new FrameworkPropertyMetadata(typeof(ClassGrid)));
    }

    public ClassGrid()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                ClassButton btn = new()
                {
                    ColorMap = _colorMap
                };
                btn.SetBinding(ClassButton.WeekProperty, new Binding(nameof(DisplayWeek))
                {
                    Source = this,
                    Mode = BindingMode.OneWay
                });
                Grid.SetColumn(btn, i + 1);
                Grid.SetRow(btn, j + 1);
                _buttons[i, j] = btn;
            }
        }
    }

    [Description("获取或设置当前日期。"), Category("公共")]
    public DateTime DateNow
    {
        get => (DateTime)GetValue(DateNowProperty);
        set => SetValue(DateNowProperty, value);
    }

    [Description("获取或设置实际学周。"), Category("公共")]
    public int ActualWeek
    {
        get => (int)GetValue(ActualWeekProperty);
        set => SetValue(ActualWeekProperty, value);
    }

    [Description("获取或设置显示学周。"), Category("公共")]
    public int DisplayWeek
    {
        get => (int)GetValue(DisplayWeekProperty);
        set => SetValue(DisplayWeekProperty, value);
    }

    [Description("获取或设置课程表数据。"), Category("公共")]
    public ClassTable? ClassTable
    {
        get => GetValue(ClassTableProperty) as ClassTable;
        set => SetValue(ClassTableProperty, value);
    }

    private Grid? GridPart { get; set; }

    private Border? IndicatorPart { get; set; }

    public override void OnApplyTemplate()
    {
        GridPart = GetTemplateChild("PART_Grid") as Grid;
        IndicatorPart = GetTemplateChild("PART_Indicator") as Border;
        if (GridPart is not null)
        {
            foreach (ClassButton btn in _buttons)
            {
                GridPart.Children.Add(btn);
            }
        }
        if (IndicatorPart is not null)
        {
            DayOfWeek day = DateNow.DayOfWeek;
            int i = day switch
            {
                DayOfWeek.Sunday => 7,
                _ => (int)day
            };
            Grid.SetColumn(IndicatorPart, i);
        }
    }

    private static void OnDateNowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ClassGrid control || control.IndicatorPart is null || e.NewValue is not DateTime now)
        {
            return;
        }
        DayOfWeek day = now.DayOfWeek;
        int i = day switch
        {
            DayOfWeek.Sunday => 7,
            _ => (int)day
        };
        Grid.SetColumn(control.IndicatorPart, i);
    }

    private static void OnActualWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ClassGrid control || e.NewValue is not int actualWeek)
        {
            return;
        }
        if (control.IndicatorPart is not null)
        {
            control.IndicatorPart.Visibility = actualWeek == control.DisplayWeek ? Visibility.Visible : Visibility.Hidden;
        }
    }

    private static void OnDisplayWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ClassGrid control || e.NewValue is not int displayWeek)
        {
            return;
        }
        if (control.IndicatorPart is not null)
        {
            control.IndicatorPart.Visibility = displayWeek == control.ActualWeek ? Visibility.Visible : Visibility.Hidden;
        }
    }

    private static void OnClassTableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ClassGrid control || e.NewValue is not ClassTable table)
        {
            return;
        }
        // 颜色分配。
        Dictionary<string, SolidColorBrush> colorMap = control._colorMap;
        colorMap.Clear();
        colorMap.Add("Inactive", s_inactiveBrush);
        IEnumerable<Course> courses = table.SelectMany(c => c);
        foreach (Course course in courses)
        {
            if (colorMap.ContainsKey(course.Title))
            {
                continue;
            }
            int i = course.Num % s_brushes.Count;
            SolidColorBrush color = s_brushes[i];
            if (colorMap.Count < s_brushes.Count)
            {
                while (colorMap.ContainsValue(color))
                {
                    color = s_brushes[++i % s_brushes.Count];
                }
            }
            colorMap.Add(course.Title, color);
        }
        // 更改课程。
        ClassButton[,] buttons = control._buttons;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                buttons[i, j].Class = table[i, j];
            }
        }
    }
}