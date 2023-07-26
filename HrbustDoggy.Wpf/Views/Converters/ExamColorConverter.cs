using Hrbust;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HrbustDoggy.Wpf.Views.Converters;

internal class ExamColorConverter : IValueConverter
{
    private static readonly Brush s_effective = new SolidColorBrush(Colors.Green);
    private static readonly Brush s_outdated = new SolidColorBrush(Colors.Gray);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Exam exam || targetType != typeof(Brush))
        {
            return DependencyProperty.UnsetValue;
        }
        return exam.Time.End > DateTime.Now ? s_effective : s_outdated;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}