using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BoardGameFrontend.Managers;

namespace BoardGameFrontend.Converters
{
    public class PlayerColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is Guid playerId && values[1] is LobbyViewModelManager lobbyManager)
            {
                return new SolidColorBrush(lobbyManager.GetColorForPlayer(playerId));
            }

            return new SolidColorBrush(Colors.Blue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not implemented for PlayerColorConverter.");
        }
    }
}