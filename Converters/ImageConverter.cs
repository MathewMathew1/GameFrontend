using System;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameFrontend
{
public class ImagePathConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imagePath)
            {
                return $"Assets/HeroCards/{imagePath}";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}