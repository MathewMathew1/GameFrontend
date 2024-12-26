using System;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameFrontend.Converters
{
    public class DivideByFourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int doubleValue && parameter is string paramString)
            {
                var parameters = paramString.Split(',');

                if (parameters.Length == 2 &&
                    double.TryParse(parameters[0], out double subtractValue) &&
                    double.TryParse(parameters[1], out double adjustmentValue))
                {
                    var dScale = 0.25f;
                    return doubleValue * dScale - adjustmentValue - subtractValue;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

