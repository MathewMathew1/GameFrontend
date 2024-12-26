using System;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameFrontend.Converters
{
    public class OverlapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index && int.TryParse(parameter?.ToString(), out int direction))
            {
                return index * direction * 20; 
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
