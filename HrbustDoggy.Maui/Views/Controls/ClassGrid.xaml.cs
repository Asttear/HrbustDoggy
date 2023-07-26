using Hrbust;

namespace HrbustDoggy.Maui.Views.Controls;

public partial class ClassGrid : ContentView
{
    public static readonly BindableProperty DateNowProperty =
        BindableProperty.Create(nameof(DateNow), typeof(DateTime), typeof(ClassGrid),
            DateTime.Now, propertyChanged: OnDateNowChanged);

    public static readonly BindableProperty ActualWeekProperty =
        BindableProperty.Create(nameof(ActualWeek), typeof(int), typeof(ClassGrid),
            0, propertyChanged: OnActualWeekChanged);

    public static readonly BindableProperty DisplayWeekProperty =
        BindableProperty.Create(nameof(DisplayWeek), typeof(int), typeof(ClassGrid),
            0, propertyChanged: OnDisplayWeekChanged);

    public static readonly BindableProperty ClassTableProperty =
        BindableProperty.Create(nameof(ClassTable), typeof(ClassTable), typeof(ClassGrid),
            null, propertyChanged: OnClassTableChanged);

    private static readonly string[] s_hexColors =
    {
        "#B71C1C", "#D81B60", "#8E24AA", "#5E35B1",
        "#3949AB", "#1E88E5", "#29B6F6", "#00ACC1",
        "#00897B", "#43A047", "#7CB342", "#C0CA33",
        "#FBC02D", "#E65100", "#F4511E", "#6D4C41"
    };

    private static readonly List<Color> s_colors = s_hexColors.Select(s => Color.FromRgba(s)).ToList();
    private static readonly Color s_inactiveColor = Colors.Gray;

    private readonly Dictionary<string, Color> _colorMap = new();
    private readonly ClassButton[,] _buttons = new ClassButton[7, 6];

    public ClassGrid()
    {
        InitializeComponent();
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
                PART_Grid.Children.Add(btn);
                _buttons[i, j] = btn;
            }
        }
        DayOfWeek day = DateNow.DayOfWeek;
        int column = day switch
        {
            DayOfWeek.Sunday => 7,
            _ => (int)day
        };
        Grid.SetColumn(PART_Indicator, column);
    }

    public DateTime DateNow
    {
        get => (DateTime)GetValue(DateNowProperty);
        set => SetValue(DateNowProperty, value);
    }

    public int ActualWeek
    {
        get => (int)GetValue(ActualWeekProperty);
        set => SetValue(ActualWeekProperty, value);
    }

    public int DisplayWeek
    {
        get => (int)GetValue(DisplayWeekProperty);
        set => SetValue(DisplayWeekProperty, value);
    }

    public ClassTable? ClassTable
    {
        get => GetValue(ClassTableProperty) as ClassTable;
        set => SetValue(ClassTableProperty, value);
    }


    private static void OnDateNowChanged(BindableObject d, object oldValue, object newValue)
    {
        if (d is not ClassGrid control || newValue is not DateTime now)
        {
            return;
        }
        DayOfWeek day = now.DayOfWeek;
        int i = day switch
        {
            DayOfWeek.Sunday => 7,
            _ => (int)day
        };
        Grid.SetColumn(control.PART_Indicator, i);
    }

    private static void OnActualWeekChanged(BindableObject d, object oldValue, object newValue)
    {
        if (d is not ClassGrid control || newValue is not int actualWeek)
        {
            return;
        }
        control.PART_Indicator.IsVisible = actualWeek == control.DisplayWeek;
    }

    private static void OnDisplayWeekChanged(BindableObject d, object oldValue, object newValue)
    {
        if (d is not ClassGrid control || newValue is not int displayWeek)
        {
            return;
        }
        control.PART_Indicator.IsVisible = displayWeek == control.ActualWeek;
    }

    private static void OnClassTableChanged(BindableObject d, object oldValue, object newValue)
    {
        if (d is not ClassGrid control || newValue is not ClassTable table)
        {
            return;
        }
        // 颜色分配。
        Dictionary<string, Color> colorMap = control._colorMap;
        colorMap.Clear();
        colorMap.Add("Inactive", s_inactiveColor);
        IEnumerable<Course> courses = table.SelectMany(c => c);
        foreach (Course course in courses)
        {
            if (colorMap.ContainsKey(course.Title))
            {
                continue;
            }
            int i = course.Num % s_colors.Count;
            Color color = s_colors[i];
            if (colorMap.Count < s_colors.Count)
            {
                while (colorMap.ContainsValue(color))
                {
                    color = s_colors[++i % s_colors.Count];
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