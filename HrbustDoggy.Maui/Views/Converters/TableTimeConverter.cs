using System.Globalization;

namespace HrbustDoggy.Maui.Views.Converters;

public class TableTimeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string)
            || values?.Length != 2
            || values[0] is not TimeOnly[] times
            || values[1] is not BindableObject bindable)
            return BindableProperty.UnsetValue;

        int pos = Grid.GetRow(bindable);
        return times[pos - 1].ToString("HH:mm");
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}