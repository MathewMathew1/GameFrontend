using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Converters
{
    public class HeroStatsConverter : IMultiValueConverter
    {
        // Dictionary to map property names to image keys
        private readonly Dictionary<string, string> _propertyToImageKeyMap = new Dictionary<string, string>
        {
            { "Morale", "Morale" },
            { "MovementFull", "MovementLeft" },
            { "MovementEmpty", "MovementUnLeft" },
            { "IncomeGold", "GoldIncome"},
            {"RoyalSignet", "Sygnet"}
        };
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return null;

            var boundObject = values[0]; // The Hero object
            var parameters = values[1].ToString()!.Split(','); // The string parameters

            var heroStats = new List<HeroStat>();

            foreach (var propertyName in parameters)
            {
                var propertyInfo = boundObject.GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(boundObject) ?? 0; // default to 0 if null

                    string imageKey = _propertyToImageKeyMap.ContainsKey(propertyName)
                        ? _propertyToImageKeyMap[propertyName]
                        : propertyName;

                    heroStats.Add(new HeroStat
                    {
                        Name = propertyName,
                        Value = (int)value,
                        ImageKey = imageKey
                    });
                }
            }

            return heroStats;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}