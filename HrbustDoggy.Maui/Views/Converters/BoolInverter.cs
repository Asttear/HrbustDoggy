using System.Globalization;

namespace HrbustDoggy.Maui.Views.Converters;

internal class BoolInverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool v || targetType != typeof(bool))
        {
            return BindableProperty.UnsetValue;
        }
        return !v;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}