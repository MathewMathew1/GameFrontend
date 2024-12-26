using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BoardGameFrontend.Converters
{
    public class ColorConnection : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isConnected)
            {
                return isConnected ? Brushes.Green : Brushes.Red; // Green if connected, Red if not
            }

            return Brushes.Transparent; // Default case
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not implemented for BooleanToColorConverter.");
        }
    }
}