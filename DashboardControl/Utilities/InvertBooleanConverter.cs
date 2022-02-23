using System;
using System.Globalization;
using System.Windows.Data;

namespace DashboardControl.Utilities
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
        {
            bool b => !b,
            _ => throw new ArgumentOutOfRangeException()
        };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value switch
        {
            bool b => !b,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
