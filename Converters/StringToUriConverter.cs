using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BoardGameFrontend.Converters
{
    public class StringToUriConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string path)
            {
                return new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            }
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}