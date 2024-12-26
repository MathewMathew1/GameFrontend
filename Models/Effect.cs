using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BoardGameFrontend.Models
{
    public class GameEffect
    {
        public required int Id { get; set; }
        public required string Type { get; set; }
        public required string TextDescription { get; set; }
        public required int Req { get; set; }
        public required int EffectType { get; set; }
        public required int IntValue1 { get; set; }
        public required int IntValue2 { get; set; }
    }

    public static class EffectsFactory
    {

        public static readonly List<GameEffect> EffectsFromJsonList;

        static EffectsFactory()
        {
            string filePath = "Data/SpecialEffect.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                EffectsFromJsonList = JsonSerializer.Deserialize<List<GameEffect>>(jsonData) ?? new List<GameEffect>();
            }
            else
            {
                EffectsFromJsonList = new List<GameEffect>();
            }
        }

        public static int? GetReqById(int id)
        {
            var effect = EffectsFromJsonList.Find(e => e.Id == id);
            return effect?.Req; 
        }
    }
}