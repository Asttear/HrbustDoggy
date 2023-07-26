using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HrbustDoggy.Wpf.Views.Converters;

internal class NullCollapser : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(Visibility))
        {
            return DependencyProperty.UnsetValue;
        }
        return value is null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}