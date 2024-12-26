using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace BoardGameFrontend.Converters
{
    public class ContainsTileConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (values.Length == 2 && values[0] is int tileID && values[1] is ObservableCollection<int> tilesWithBorder)
            {
                if (tilesWithBorder.Contains(tileID))
                {
                    return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}