using System.Globalization;

namespace HrbustDoggy.Maui.Views.Converters;

internal class TableTitleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string) || value is not int displayWeek)
        {
            return BindableProperty.UnsetValue;
        }
        return displayWeek > 0 ? $"第{displayWeek}周" : "未开学";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
