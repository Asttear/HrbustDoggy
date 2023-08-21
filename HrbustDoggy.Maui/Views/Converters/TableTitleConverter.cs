using System.Globalization;

namespace HrbustDoggy.Maui.Views.Converters;

internal class TableTitleConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string))
        {
            return BindableProperty.UnsetValue;
        }
        if (values[0] is null || values[1] is not int week)
        {
            return "无数据";
        }
        return week > 0 ? $"第{week}周" : "未开学";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
