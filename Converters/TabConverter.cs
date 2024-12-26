using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Converters
{
    public class TabHighlightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Tab selectedTab && parameter is Tab tabString)
            {
                
                    return selectedTab == tabString ? Brushes.Green : Brushes.Gray;               
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}