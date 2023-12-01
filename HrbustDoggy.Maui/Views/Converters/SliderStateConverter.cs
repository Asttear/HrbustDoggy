using System.Globalization;

namespace HrbustDoggy.Maui.Views.Converters;

internal class SliderStateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (targetType != typeof(bool) || value is not int actualWeek)
        {
            return BindableProperty.UnsetValue;
        }
        return actualWeek > 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
