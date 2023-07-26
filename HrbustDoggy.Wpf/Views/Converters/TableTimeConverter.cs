using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HrbustDoggy.Wpf.Views.Converters;

public class TableTimeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string)
            || values?.Length != 2
            || values[0] is not TimeOnly[] times
            || values[1] is not int pos)
            return DependencyProperty.UnsetValue;

        return times[pos - 1].ToString("HH:mm");
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}