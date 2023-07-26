using Hrbust;
using System.Globalization;

namespace HrbustDoggy.Maui.Views.Converters;

internal class ExamColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Exam exam)
        {
            return Colors.Gray;
        }
        return exam.Time.End > DateTime.Now ? Colors.Green : Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}