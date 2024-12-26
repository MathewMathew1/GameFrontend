using System;
using System.Windows;
using System.Windows.Data;

namespace BoardGameFrontend.Converters
{
    public class RectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 4 &&
                values[0] is double offsetX &&
                values[1] is double offsetY &&
                values[2] is double width &&
                values[3] is double height)
            {
                return new Rect(offsetX, offsetY, width, height);
            }
            return Rect.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}