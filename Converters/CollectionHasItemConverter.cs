using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BoardGameFrontend.Converters
{
    public class CollectionHasItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Collections.IEnumerable collection)
            {
                foreach (var _ in collection) 
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
