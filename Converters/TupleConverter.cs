using System;
using System.Globalization;
using System.Windows.Data;


namespace BoardGameFrontend.Converters
{
    public class TupleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Assuming the first is a boolean and second is your complex LobbyService
            return Tuple.Create(values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
