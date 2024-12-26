using System;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameFrontend.Converters
{
    public class GreaterThanOrEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int number && int.TryParse(parameter.ToString(), out int comparisonValue))
            {
                return number >= comparisonValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}