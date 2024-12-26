using System;
using System.Globalization;
using System.Windows.Data;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Converters
{
    public class EnumToFriendlyNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TurnTypes turnType)
            {
                return turnType switch
                {
                    TurnTypes.PHASE_BY_PHASE => "Phase by Phase",
                    TurnTypes.FULL_TURN => "Full Turn",
                    _ => value.ToString() // fallback to enum name if needed
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue switch
                {
                    "Phase by Phase" => TurnTypes.PHASE_BY_PHASE,
                    "Full Turn" => TurnTypes.FULL_TURN,
                    _ => Binding.DoNothing
                };
            }
            return Binding.DoNothing;
        }
    }
}
