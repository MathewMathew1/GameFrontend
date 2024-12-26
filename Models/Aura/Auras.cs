using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BoardGameFrontend.Models
{  
    public enum AurasType
    {
        RETURN_TO_CENTER_ON_MOVEMENT,
        ONE_EMPTY_MOVEMENT,
        ONE_FULL_MOVEMENT,
        EMPTY_MOVES_INTO_FULL,
        NO_FRACTION_MOVEMENT,
        FULFILL_PROPHECY,
        TELEPORTATION_REWARD_ONE_FREE_MOVEMENT,
        BUY_CARDS_BY_ANY_RESOURCE,
        BLOCK_TILE,
        ADD_ADDITIONAL_MOVEMENT_BASED_ON_FRACTION,
        MAKE_CHEAPER_MERCENARIES,
        GOLD_ON_TILES_WITHOUT_GOLD,
        GOLD_ON_TILES_WITH_IRON,
        GOLD_ON_TILES_WITH_WOOD,
        GOLD_ON_TILES_WITH_NITER,
        GOLD_ON_TILES_WITH_MYSTIC_FOG,
        GOLD_ON_TILES_WITH_GEMS,
        GOLD_ON_TILES_WITH_REROLL,
        GOLD_ON_TILES_WITH_ARTIFACT,
        GOLD_ON_TILES_WITH_SIGNET,
        GOLD_ON_TILE_TELEPORT,
        EMPTY_MOVE_ON_TILES_WITH_SIGNET,
        ARTIFACT_ON_ROYAL_CARD,
        FULL_MOVEMENT_INTO_EMPTY,
        GOLD_FOR_MOVEMENT,
        TEMPORARY_SIGNET,
        ARTIFACT_WHEN_CLOSE_TO_CASTLE,
        FULL_MOVE_WHEN_CLOSE_TO_CASTLE,
        EMPTY_MOVE_WHEN_CLOSE_TO_CASTLE,
        GOLD_WHEN_CLOSE_TO_CASTLE,
        CHEAPER_BUILDINGS,
        CHANGE_SIDES_OF_HERO_AFTER_PLAY,
        REPLACE_NEXT_HERO,
        EMPTY_MOVEMENT_WHEN_HERO_HAS_SIGNET,
        EMPTY_MOVEMENT_WHEN_HERO_HAS_MORALE,
        EMPTY_MOVEMENT_WHEN_HERO_HAS_EMPTY_MOVEMENT,
        ADJACENT_TILE_REWARD,
        EMPTY_MOVE_ON_TILES_WITH_TELEPORT,
        GOLD_WHEN_NO_GOLD,
        EMPTY_MOVE_ON_TILE_WITH_CASTLE,
        EXTRA_ARTIFACT_REROLL,
        INSTANT_WIN_DUEL,
        GOLD_ON_TILE_DUEL,
        EMPTY_MOVEMENT_WHEN_HERO_HAS_POINT
    }

    public enum EndGameAuraType
    {
        CUMMULATIVE_POINTS,   
        SIGNETS_INTO_POINTS,
        THREE_POINTS,
        POINTS_OF_MERCENARY_OF_FACTION 
    }

    public class EndGameAura
    {
        public EndGameAuraType Aura {get; set;}
        public int? Value1 {get; set;}  
    }

    public class AuraTypeWithLongevity{
        public AurasType Aura {get; set;}
        public int? Value1 {get; set;}
        public bool Permanent {get; set;}
    }

    public class AddAura{
        public AuraTypeWithLongevity Aura {get; set;}
        public Guid PlayerId {get; set;}
    }

    
    public class AuraInfoDisplay{
        public required int Id {get; set;}
        public required string Type {get; set;}
        public required string ToolTipText {get; set;}
        public required string ImageAtlas {get; set;}
        public required List<int> ImageIndex {get; set;}
    }

    public class AuraExtendedInfoDisplay{
        public int Id {get; set;}
        public string Type {get; set;}
        public string ToolTipText {get; set;}
        public string ImageAtlas {get; set;}
        public int ImageIndex {get; set;}
        public DisplayInfo DisplayInfo { get; set; } = new DisplayInfo();

        public AuraExtendedInfoDisplay(AuraInfoDisplay auraInfoDisplay, int imageIndex){
            Id = auraInfoDisplay.Id;
            Type = auraInfoDisplay.Type;
            ToolTipText = auraInfoDisplay.ToolTipText;
            ImageAtlas = auraInfoDisplay.ImageAtlas;
            ImageIndex = auraInfoDisplay.ImageIndex[Math.Min(imageIndex-1, auraInfoDisplay.ImageIndex.Count-1)];
            DisplayInfo = new DisplayInfo();
            DisplayInfo.SetupData(ImageAtlas, ImageIndex);
        }
    }

    public static class AuraFactory
    {

        public static readonly List<AuraInfoDisplay> AurasFromJson;

        static AuraFactory()
        {
            string filePath = "Data/Auras.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                AurasFromJson = JsonSerializer.Deserialize<List<AuraInfoDisplay>>(jsonData) ?? new List<AuraInfoDisplay>();
            }
            else
            {
                AurasFromJson = new List<AuraInfoDisplay>();
            }
        }

        public static AuraExtendedInfoDisplay GetAuraDisplayInfo(AurasType aura, int imageIndex){
            var auraFromJson = AurasFromJson.FirstOrDefault(a => a.Type == aura.ToString()) ?? AurasFromJson[0];
            AuraExtendedInfoDisplay auraExtendedInfoDisplay = new AuraExtendedInfoDisplay(auraFromJson, imageIndex);

            return auraExtendedInfoDisplay;
        }
    }

}