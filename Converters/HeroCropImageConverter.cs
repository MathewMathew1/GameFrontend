using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Converters
{
    public class HeroCropImageConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string name)
            {
                var IconAtlas = HeroStatsFactory.GetByName(name).ImagePathString;
                return new BitmapImage(new Uri(IconAtlas, UriKind.RelativeOrAbsolute));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}