using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Converters
{
    public class GoldAmountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3)
                return string.Empty;

            var resourceInfo = values[0] as ResourceInfoData;
            var mercenary = values[1] as MercenaryDisplay;
            var auraDiscount = values[2] as int? ?? 0;

            if (resourceInfo != null && mercenary != null)
            {
                if (resourceInfo.Name == ResourceType.Gold)
                {
                    var goldAfterDiscount = Math.Max(0, resourceInfo.Amount - mercenary.GoldDecrease - auraDiscount);
                    var textString = goldAfterDiscount != resourceInfo.Amount
                        ? $"{goldAfterDiscount} ({resourceInfo.Amount})"
                        : resourceInfo.Amount.ToString();
                    return textString;
                }
                return "";
            }

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported.");
        }
    }

    public class GoldColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return Brushes.White;

            var resourceInfo = values[0] as ResourceInfoData;
            var mercenary = values[1] as MercenaryDisplay;

            if (resourceInfo != null && mercenary != null)
            {
                if (resourceInfo.Name == ResourceType.Gold && (mercenary.GoldDecrease > 0 || mercenary.AuraDiscount > 0))
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8bf205"));
                }
            }

            return Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported.");
        }
    }
}

