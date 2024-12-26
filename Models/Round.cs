
using System.Collections.Generic;

namespace BoardGameFrontend.Models
{
    public class EndOfRoundMercenaryData
    {
        public required List<Mercenary> Mercenaries { get; set; }
        public required MercenariesLeftData MercenariesLeftData { get; set; }
    }

    public class EndOfRoundData
    {
        public required EndOfRoundMercenaryData EndOfRoundMercenaryData { get; set; }
    }
}