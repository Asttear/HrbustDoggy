using System.Globalization;

namespace HrbustDoggy.Maui.Views.Converters;

public class TableDateConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string)
            || values?.Length != 4
            || values[0] is not DateTime now
            || values[1] is not int actualWeek
            || values[2] is not int displayWeek
            || values[3] is not BindableObject bindable)
        {
            return BindableProperty.UnsetValue;
        }

        int days = 7 * (displayWeek - actualWeek);
        int dayOfWeek = now.DayOfWeek switch
        {
            DayOfWeek.Sunday => 7,
            _ => (int)now.DayOfWeek
        };
        int pos = Grid.GetColumn(bindable);
#if WINDOWS
        return now.AddDays(days + pos - dayOfWeek).ToString("yyyy/MM/dd");
#else
        return now.AddDays(days + pos - dayOfWeek).ToString("MM/dd");
#endif
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}