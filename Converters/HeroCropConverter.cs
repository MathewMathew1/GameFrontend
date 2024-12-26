using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Converters
{
    public class HeroCropConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string name)
            {
                return HeroStatsFactory.GetByName(name).CropRect;
            }

            return new Int32Rect(
                256,
                256,
                256,
                256
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}