using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace BoardGameFrontend.Converters
{
    public class MatchToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0]?.Equals(values[1]) == true)
            {
                if (parameter is string colorString)
                {
                    return (SolidColorBrush)(new BrushConverter().ConvertFromString(colorString))!;
                }
                return Brushes.Green; 
            }
            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}