using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Converters
{
    public class CombinedConditionsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Ensure we have the expected number of values
            if (values.Length < 3) return Visibility.Collapsed;

            if (values[1] is ObservableCollection<AuraTypeWithLongevity> aurasTypes && values[2] is AurasType auraType && values[3] is Phase phase)
            {
                bool isUserTurn = (bool)values[0];

                bool hasAura = aurasTypes.Any(aura => aura.Aura == auraType);

                bool phaseIsGood = phase.Name == PhaseType.BoardPhase;

                return isUserTurn && hasAura && phaseIsGood ? Visibility.Visible: Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
