using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BoardGameFrontend.Helpers
{
    public class EmptySlotColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() == "Empty Slot")
            {
                return Brushes.Gray;
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}