using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BoardGameFrontend.Models
{
    public class EndOfGame
    {
        public required Dictionary<Guid, ScorePointsTable> PlayerScores { get; set; }
        public required Dictionary<Guid, TimeSpan> PlayerTimeSpan { get; set; }
        public TimeSpan GameTimeSpan { get; set; }
    }

    public class EndOfGameWithPlayers
    {
        public required List<ScorePointsTableWithPlayer> PlayerScores { get; set; }
        public required Dictionary<Guid, TimeSpan> PlayerTimeSpan { get; set; }
        public TimeSpan GameTimeSpan { get; set; }
    }

    public class StartOfGame
    {
        public required MercenaryData MercenaryData { get; set; }
        public required List<PlayerViewModelData> Players { get; set; }
        public required string GameId { get; set; }
        public required List<TokenTileInfo> TokenSetup { get; set; }
        public required List<RolayCard> RolayCards { get; set; }
    }

    public class StartGameModel
    {
        [JsonPropertyName("turnType")]
        public TurnTypes TurnType { get; set; } = TurnTypes.FULL_TURN;
        [JsonPropertyName("lessCards")]
        public bool LessCards { get; set; } = false;
        [JsonPropertyName("moreHeroCards")]
        public bool MoreHeroCards { get; set; } = false;
        [JsonPropertyName("removePropheciesAtLastRound")]
        public bool RemovePropheciesAtLastRound { get; set; } = false;
        [JsonPropertyName("sameAmountOfMercenariesEachRound")]
        public bool SameAmountOfMercenariesEachRound {get; set;} = false;
    }

    public class GameOptions
    {
        public bool AutomaticSwitchBetweenTabs { get; set; } = true;

        private static readonly string FilePath = "gameOptions.json";

        public static GameOptions LoadFromFile()
        {
            if (File.Exists(FilePath))
            {
                string jsonData = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<GameOptions>(jsonData) ?? new GameOptions();
            }
            else
            {
                return new GameOptions();
            }
        }

        public void SaveToFile()
        {
            string jsonData = JsonSerializer.Serialize(this);
            File.WriteAllText(FilePath, jsonData);
        }
    }

    public class FullGameData
    {
        public required int Turn { get; set; }
        public required int Round { get; set; }
        public required PhaseType CurrentPhase { get; set; }
        public MiniPhaseType? CurrentMiniPhase { get; set; }
        public required MercenaryData MercenaryData { get; set; }
        public required List<FullPlayerData> PlayersData { get; set; }
        public required string GameId { get; set; }
        public required List<TokenTileInfo> TokenSetup { get; set; }
        public required List<RolayCard> RoyalCards { get; set; }
        public required List<HeroCardCombined> HeroCards { get; set; }
        public required ArtifactInfo ArtifactInfo { get; set; }
        public required List<Player> PlayerBasedOnMorales { get; set; }
        public required int PawnTilePosition { get; set; }
        public required Guid CurrentPlayerId { get; set; }
    }

    public class FullPlayerData
    {
        public required Player Player { get; set; }
        public required List<AuraTypeWithLongevity> Auras { get; set; }
        public required Dictionary<ResourceType, int> Resources { get; set; }
        public required Dictionary<ResourceHeroType, int> ResourceHero { get; set; }
        public required List<Mercenary> Mercenaries { get; set; }
        public required ArtifactPlayerData Artifacts { get; set; }
        public required RoyalCardsPlayerData RoyalCardsData { get; set; }
        public required PlayerHeroData Heroes { get; set; }
        public required List<Token> Tokens { get; set; }
        public required int Morale { get; set; }
        public required int GoldIncome { get; set; }
        public required bool AlreadyPlayedTurn { get; set; }
        public required List<string> BoolStorage {get; set;}
    }
}