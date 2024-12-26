using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using BoardGameFrontend.Models; // Assuming models are in this namespace

namespace BoardGameFrontend.Converters
{
    public class ReqConverter : IValueConverter
    {
        // You can create a dictionary mapping some properties to ImageKeys, if needed
        private readonly Dictionary<int, string> _otherReqs = new Dictionary<int, string>
        {
            { 1, "Siege" },
            { 6, "Magic" },
            { 7, "Army" },
            { 8, "Faction1" },
            { 9, "Faction2" },
            { 10, "Faction3" },
            { 11, "Faction4" },
        };


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MercenaryDisplay mercenaryDisplay && mercenaryDisplay.Req != null)
            {

                var visible = mercenaryDisplay.Req != -1;
                int? valueData = null;

                var imageKey = _otherReqs.ContainsKey(mercenaryDisplay.Req.Value)
               ? _otherReqs[mercenaryDisplay.Req.Value]
               : "defaultImageKey";

                var requirement = RequirementMovementStore.GetRequirementById(mercenaryDisplay.Req.Value);
                if (requirement != null)
                {
                    valueData = requirement.Value1;
                }

                if(mercenaryDisplay.Req.Value > 7 && mercenaryDisplay.Req.Value < 12){
                    valueData = null;
                }

                var result = new ReqConverterData
                {
                    Visible = visible,
                    Value = valueData,
                    ImageKey = imageKey
                };

                return result;
            }

            return new ReqConverterData
            {
                Visible = false,
                Value = 0,
                ImageKey = "Siege"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}