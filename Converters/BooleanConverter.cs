using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BoardGameFrontend.Converters
{
    public class BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter) ? Brushes.Green : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}