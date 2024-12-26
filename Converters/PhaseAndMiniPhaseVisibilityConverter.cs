using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Converters
{
    public class PhaseAndMiniPhaseVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 4) return Visibility.Collapsed;

            bool isUserControlledTurn = values[0] is bool userControlled && userControlled;
            string phase = values[1]?.ToString();
            var miniPhase = values[2] as MiniPhase;
            var collection = values[3] as System.Collections.IEnumerable;

            if (isUserControlledTurn && (phase == PhaseType.ArtifactPhase.ToString() || miniPhase?.Name == MiniPhaseType.ArtifactPickPhase))
            {
                if (collection != null)
                {
                    foreach (var item in collection)
                    {
                        return Visibility.Visible;
                    }
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
