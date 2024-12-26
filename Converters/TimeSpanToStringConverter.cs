using System;
using System.Globalization;
using System.Windows.Data;
using BoardGameFrontend.Helpers;


namespace BoardGameFrontend.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                // Use the TimeFormatHelper to format the TimeSpan
                return TimeFormatHelper.FormatTimeSpan(timeSpan);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not supported.");
        }
    }
}