using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameFrontend.Converters
{
    public class ContainsStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string stringToLookFor && value is ObservableCollection<string> list)
            {
                return list.Contains(stringToLookFor);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}