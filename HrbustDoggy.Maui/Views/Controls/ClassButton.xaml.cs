using CommunityToolkit.Mvvm.Input;
using Hrbust;

namespace HrbustDoggy.Maui.Views.Controls;

public partial class ClassButton : ContentView
{
    public static readonly BindableProperty ClassProperty =
        BindableProperty.Create(nameof(Class), typeof(Class), typeof(ClassButton), null, propertyChanged: OnClassChanged);

    public static readonly BindableProperty WeekProperty =
        BindableProperty.Create(nameof(Week), typeof(int), typeof(ClassButton), 0, propertyChanged: OnWeekChanged);

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(ClassButton), Colors.Red);

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ClassButton), Colors.White);

    public static readonly BindableProperty ColorMapProperty =
        BindableProperty.Create(nameof(ColorMap), typeof(Dictionary<string, Color>), typeof(ClassButton), null);

    public ClassButton()
    {
        InitializeComponent();
        PART_Button.Command = new AsyncRelayCommand(ShowClassInfoAsync);
    }

    public Color? TextColor
    {
        get => GetValue(TextColorProperty) as Color;
        set => SetValue(TextColorProperty, value);
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

    public Color? AccentColor
    {
        get => GetValue(AccentColorProperty) as Color;
        set => SetValue(AccentColorProperty, value);
    }


    public Dictionary<string, Color>? ColorMap
    {
        get => GetValue(ColorMapProperty) as Dictionary<string, Color>;
        set => SetValue(ColorMapProperty, value);
    }

    private static void OnClassChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ClassButton control)
        {
            return;
        }
        if (newValue is not Class aClass || aClass.Count == 0)
        {
            control.IsVisible = false;
        }
        else
        {
            control.IsVisible = true;
            control.PART_Mark.IsVisible = aClass.Count > 1;
            control.Update();
        }
    }

    private static void OnWeekChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ClassButton control)
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
        PART_Accent.IsVisible = actives.Count > 1;
        if (actives.Count == 0)
        {
            PART_Title.Text = "本周无课程";
            PART_Location.IsVisible = false;
            PART_Button.BackgroundColor = ColorMap?["Inactive"];
        }
        else
        {
            PART_Title.Text = actives[0].Title;
            PART_Location.IsVisible = true;
            PART_Location.Text = actives[0].Location;
            PART_Button.BackgroundColor = ColorMap?[actives[0].Title];
        }
    }

    private async Task ShowClassInfoAsync()
    {
        if (Class is null || Class.Count == 0)
        {
            await Shell.Current.DisplayAlert("课程信息", "暂无课程信息。", "确定");
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
        await Shell.Current.DisplayAlert("课程信息", string.Join("\n\n", courseInfos), "确定");
    }
}