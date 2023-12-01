using System.Globalization;

namespace HrbustDoggy.Maui.Views.Converters;

internal class NullToFalseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType != typeof(bool))
        {
            return BindableProperty.UnsetValue;
        }
        return value is not null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}