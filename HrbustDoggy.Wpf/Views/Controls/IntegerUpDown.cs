using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;

namespace HrbustDoggy.Wpf.Views.Controls;

public class IntegerUpDown : Control
{
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(IntegerUpDown), new PropertyMetadata(0, OnValueChanged));

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(IntegerUpDown), new PropertyMetadata(0, OnValueChanged));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(int), typeof(IntegerUpDown),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));

    private readonly RelayCommand _increaseCommand;

    private readonly RelayCommand _decreaseCommand;

    private Button? _increasePart;

    private Button? _decreasePart;

    static IntegerUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(IntegerUpDown), new FrameworkPropertyMetadata(typeof(IntegerUpDown)));
    }

    public IntegerUpDown()
    {
        _increaseCommand = new(() => Value++, () => Value < Maximum);
        _decreaseCommand = new(() => Value--, () => Value > Minimum);
    }

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private Button? IncreasePart
    {
        set
        {
            _increasePart = value;
            if (_increasePart is not null)
            {
                _increasePart.Command = _increaseCommand;
            }
        }
    }

    private Button? DecreasePart
    {
        set
        {
            _decreasePart = value;
            if (_decreasePart is not null)
            {
                _decreasePart.Command = _decreaseCommand;
            }
        }
    }

    public override void OnApplyTemplate()
    {
        //InputPart = GetTemplateChild("PART_Input") as TextBox;
        IncreasePart = GetTemplateChild("PART_Increase") as Button;
        DecreasePart = GetTemplateChild("PART_Decrease") as Button;
    }

    private static object CoerceValue(DependencyObject d, object baseValue)
    {
        if (d is not IntegerUpDown control || baseValue is not int value)
        {
            return DependencyProperty.UnsetValue;
        }
        if (value > control.Maximum)
        {
            return control.Maximum;
        }
        else if (value < control.Minimum)
        {
            return control.Minimum;
        }
        return value;
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not IntegerUpDown control)
        {
            return;
        }
        control._increaseCommand.NotifyCanExecuteChanged();
        control._decreaseCommand.NotifyCanExecuteChanged();
    }
}