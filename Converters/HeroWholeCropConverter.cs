using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Converters
{
    public class HeroWholeCropConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var resource = values[0];

            if (resource == null)
            {
                return null;
            }

            var heroStats = HeroStatsFactory.GetByName(resource.ToString()!);

            if (heroStats == null)
                return null;

            BitmapImage bitmapImage = new BitmapImage(new Uri(heroStats.ImagePathString, UriKind.RelativeOrAbsolute));
            var cropRect = heroStats.CropRect;

            return new CroppedBitmap(bitmapImage, new Int32Rect((int)cropRect.X, (int)cropRect.Y, (int)cropRect.Width, (int)cropRect.Height));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}