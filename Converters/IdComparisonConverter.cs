using System;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameFrontend.Converters
{
    public class IdComparisonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is int selectedId && values[1] is int currentId)
            {
                return selectedId == currentId;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}