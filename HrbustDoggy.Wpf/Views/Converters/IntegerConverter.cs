using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HrbustDoggy.Wpf.Views.Converters;

internal class IntegerConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int v || targetType != typeof(string))
        {
            return DependencyProperty.UnsetValue;
        }
        return v.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string v || targetType != typeof(int))
        {
            return DependencyProperty.UnsetValue;
        }
        if (!int.TryParse(v, out int result))
        {
            return DependencyProperty.UnsetValue;
        }
        return result;
    }
}
