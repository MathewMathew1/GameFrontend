using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Converters
{
    public class HasAuraConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is ObservableCollection<AuraTypeWithLongevity> aurasTypes) || values[1] == null)
                return true;

            if (values[1] is AurasType auraType)
            {
                var show = aurasTypes.Any(aura => aura.Aura == auraType);
                return true;
            }


            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}