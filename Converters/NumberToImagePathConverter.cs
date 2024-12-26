using System;
using System.Globalization;
using System.Windows.Data;

namespace BoardGameFrontend.Converters
{
    public class NumberToImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int number)
            {
                string basePath = "pack://application:,,,/Assets/Helpers/";
                return number switch
                {
                    <= 2 => $"{basePath}BattleHelper2orLess.png",
                    3 => $"{basePath}BattleHelper3.png",
                    4 => $"{basePath}BattleHelper4.png",
                    _ => $"{basePath}DefaultBattleHelper.png"
                };
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack is not supported in NumberToImagePathConverter.");
        }
    }
}