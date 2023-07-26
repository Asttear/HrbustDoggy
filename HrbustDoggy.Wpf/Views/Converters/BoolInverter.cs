using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HrbustDoggy.Wpf.Views.Converters;

internal class BoolInverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool v || targetType != typeof(bool))
        {
            return DependencyProperty.UnsetValue;
        }
        return !v;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}