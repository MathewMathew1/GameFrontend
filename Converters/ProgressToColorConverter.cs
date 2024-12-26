using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BoardGameFrontend.Converters
{
    public class ProgressToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double progress)
            {
                // Progress-based color logic
                if (progress <= 33)
                    return Brushes.Red;
                else if (progress <= 66)
                    return Brushes.Orange;
                else
                    return Brushes.Green;
            }
            return Brushes.Gray; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}