using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using BoardGameFrontend.VisualManager;

namespace BoardGameFrontend.Models
{
    public class ResourceInfoData
    {
        public required ResourceType Name { get; set; }
        public required int Amount { get; set; }
    }

    public class FulfillProphecy
    {
        public required int MercenaryId { get; set; }
        public required Guid PlayerId { get; set; }
    }

    public class Mercenary
    {
        public string? ToolTipText {get; set;}
        public required int Id { get; set; }
        public required string NameEng { get; set; }
        public required int Siege { get; set; }
        public required int Magic { get; set; }
        public required int Army { get; set; }
        public required int TypeCard { get; set; }
        public required int Morale { get; set; }
        public required int IncomeGold { get; set; }
        public required Fraction Faction { get; set; }
        public int? Req { get; set; }
        public int InGameIndex { get; set; }
        public required List<ResourceInfoData> ResourcesNeeded { get; set; }
        public required int GoldDecrease { get; set; }
        public required string BackgroundAtlas { get; set; }
        public required int BackgroundIndex { get; set; }
        public required int ScorePoints { get; set; }
        public LockedByPlayerInfo? LockedByPlayerInfo { get; set; }
        public string? EffectIconAtlas { get; set; }
        public int? EffectIconIndex { get; set; }
        public int? EffectId { get; set; }
        

        public int? EffectType { get; set; }
    }

    public class MercenaryTypeCard
    {
        public required int Id { get; set; }
        public required string NameType { get; set; }
        public required int IconIndex { get; set; }
        public required string IconAtlas { get; set; }
    }

    public class LockMercenaryData{
        public required LockedByPlayerInfo LockMercenary {get; set;}
        public required int MercenaryId {get; set;}
    }

    public class EffectImage
    {
        public string BackgroundAtlas { get; set; }
        public int BackgroundIndex;
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string ImagePathString { get; set; } = "";
        public int IconWidth { get; set; } = 128;
        public int IconHeight { get; set; } = 128;

        public EffectImage(string AtlasPath, int AtlasId)
        {
            BackgroundAtlas = AtlasPath;
            BackgroundIndex = AtlasId;
            SetupData();
        }

        public void SetupData()
        {
            if (!string.IsNullOrEmpty(BackgroundAtlas))
            {
                var iconStruct = IconAtlases.GetIconAtlasByNameType(BackgroundAtlas).GetImageInfo(BackgroundIndex);

                OffsetX = iconStruct.OffsetX;
                OffsetY = iconStruct.OffsetY;
                ImagePathString = iconStruct.ImagePath;
                IconWidth = iconStruct.IconWidth;
                IconHeight = iconStruct.IconHeight;
            }
        }

        public Int32Rect CropRect => new Int32Rect(
            OffsetX,
            OffsetY,
            IconWidth,
            IconHeight
        );
    }

    public class MercenaryDisplay : INotifyPropertyChanged
    {
        public string? ToolTipText {get; set;}
        public string? EffectIconAtlas { get; set; }
        public int? EffectIconIndex { get; set; }
        public int? EffectId { get; set; }
        public int? Req { get; set; }
        public int? EffectType { get; set; }
        public EffectType EffectTypeClass { get; set; }
        public EffectImage EffectImage { get; set; }
        public required int Id { get; set; }
        public required string NameEng { get; set; }
        public required int Siege { get; set; }
        public required int Magic { get; set; }
        public required int Army { get; set; }
        public required int TypeCard { get; set; }
        private int _auraDiscount = 0;
        public int AuraDiscount
        {
            get => _auraDiscount;
            set
            {
                if (_auraDiscount != value)
                {
                    _auraDiscount = value;
                    OnPropertyChanged(nameof(AuraDiscount));
                }
            }
        }
        private LockedByPlayerInfo? _lockedByPlayerInfo;
        public LockedByPlayerInfo? LockedByPlayerInfo
        {
            get => _lockedByPlayerInfo;
            set
            {
                if (_lockedByPlayerInfo != value)
                {
                    _lockedByPlayerInfo = value;
                    OnPropertyChanged(nameof(LockedByPlayerInfo));
                }
            }
        }
        public HeroStatsWithAtlas TypeCardName { get; set; }
        public required int Morale { get; set; }
        public required int IncomeGold { get; set; }
        public required int ScorePoints { get; set; }
        public required Fraction Faction { get; set; }
        public int InGameIndex { get; set; }
        public required List<ResourceInfoData> ResourcesNeeded { get; set; }
        public required string BackgroundAtlas { get; set; }
        private int _backgroundIndex;
        public int BackgroundIndex
        {
            get => _backgroundIndex;
            set
            {
                _backgroundIndex = value;
                SetupData();
            }
        }
        private bool _prophecyRequirementsFulfilled;
        public bool ProphecyRequirementsFulfilled
        {
            get => _prophecyRequirementsFulfilled;
            set
            {
                _prophecyRequirementsFulfilled = value;
                OnPropertyChanged(nameof(ProphecyRequirementsFulfilled));
            }
        }
        private bool _isAlwaysFulfilled;
        public bool IsAlwaysFulfilled
        {
            get => _isAlwaysFulfilled;
            set
            {
                _isAlwaysFulfilled = value;
                OnPropertyChanged(nameof(IsAlwaysFulfilled));
            }
        }
        public required int OffsetX { get; set; }
        public required int OffsetY { get; set; }
        public string ImagePathString { get; set; } = "";
        public int IconWidth { get; set; } = 128;
        public int IconHeight { get; set; } = 128;

        public void SetupData()
        {
            if (!string.IsNullOrEmpty(BackgroundAtlas))
            {
                var iconStruct = IconAtlases.GetIconAtlasByNameType(BackgroundAtlas).GetImageInfo(BackgroundIndex);

                OffsetX = iconStruct.OffsetX;
                OffsetY = iconStruct.OffsetY;
                ImagePathString = iconStruct.ImagePath;
                IconWidth = iconStruct.IconWidth;
                IconHeight = iconStruct.IconHeight;
                TypeCardName = MercenaryTypesFactory.GetMercenaryTypeById(TypeCard);
                if (EffectType != null) EffectTypeClass = new EffectType(EffectType.Value);
                if (EffectIconAtlas != null && EffectIconIndex != null) EffectImage = new EffectImage(EffectIconAtlas, EffectIconIndex.Value);
            }
        }

        public Int32Rect CropRect => new Int32Rect(
            OffsetX,
            OffsetY,
            IconWidth,
            IconHeight
        );

        private int _goldDecrease = 0;
        public int GoldDecrease
        {
            get => _goldDecrease;
            set
            {
                if (_goldDecrease != value)
                {
                    _goldDecrease = value;
                    OnPropertyChanged(nameof(GoldDecrease));
                }
            }
        }

        private bool _selected = false;
        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged(nameof(Selected));
                }
            }
        }

        private bool _canBuy = false;
        public bool CanBuy
        {
            get => _canBuy;
            set
            {
                if (_canBuy != value)
                {
                    _canBuy = value;
                    OnPropertyChanged(nameof(CanBuy));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MercenaryPickedData
    {
        public Reward? Reward {get; set;}
        public required Mercenary Card { get; set; }
        public required PlayerViewModel Player { get; set; }
        public required List<ResourceInfoData> ResourcesSpend { get; set; }
        public required Mercenary? MercenaryReplacement { get; set; }
        public required MercenariesLeftData MercenariesLeftData { get; set; }
    }

    public class MercenaryRerolledData
    {
        public required Mercenary Card { get; set; }
        public required MercenariesLeftData MercenariesLeftData { get; set; }
    }

    public class PreMercenaryRerolled
    {
        public required Mercenary? MercenaryReplacement { get; set; }
        public required MercenariesLeftData MercenariesLeftData { get; set; }
    }

    public class MercenariesLeftData
    {
        public required int TossedMercenariesAmount { get; set; }
        public required int MercenariesAmount { get; set; }
    }

    public class MercenaryData
    {
        [JsonPropertyName("buyableMercenaries")]
        public required List<Mercenary> BuyableMercenaries { get; set; }
        [JsonPropertyName("remainingMercenariesAmount")]
        public int RemainingMercenariesAmount { get; set; }
        [JsonPropertyName("tossedMercenariesAmount")]
        public required int TossedMercenariesAmount {get; set;}
    }




    public static class MercenaryTypesFactory
    {

        public static readonly List<MercenaryTypeCard> ResourceFromJsonList = new List<MercenaryTypeCard>();
        public static readonly List<HeroStatsWithAtlas> ResourceFromJsonListWithEng = new List<HeroStatsWithAtlas>();

        static MercenaryTypesFactory()
        {
            string filePath = "Data/RedCards.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                ResourceFromJsonList = JsonSerializer.Deserialize<List<MercenaryTypeCard>>(jsonData) ?? new List<MercenaryTypeCard>();
                ResourceFromJsonList.ForEach(resource =>
                {
                    ResourceFromJsonListWithEng.Add(new HeroStatsWithAtlas(resource));
                });
            }
        }

        public static HeroStatsWithAtlas GetMercenaryTypeById(int id)
        {
            return ResourceFromJsonListWithEng.FirstOrDefault(mercenaryCardType => mercenaryCardType.Id == id)!;
        }

    }

    public class BuyableMercenariesRefreshed
    {
        public required List<Mercenary> NewBuyableMercenaries { get; set; }
        public required MercenariesLeftData MercenariesLeftData { get; set; }
    }
}