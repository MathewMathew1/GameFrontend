using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using BoardGameFrontend.VisualManager;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BoardGameFrontend.Models
{
    public class HeroCard
    {
        public required int Id { get; set; }
        public required string HeroName { get; set; }
        public required int Army { get; set; }
        public required int Magic { get; set; }
        public required int Siege { get; set; }
        public required int Morale { get; set; }
        public required Fraction Faction { get; set; }
        public required int MovementFull { get; set; }
        public required int MovementEmpty { get; set; }
        public required int ScorePoints { get; set; }
        public required int ImageIndex { get; set; }
        public required string ImageAtlas { get; set; }
        public required int RoyalSignet { get; set; }
        public string? EffectImageAtlas { get; set; }
        public int? EffectImageIndex { get; set; }
        public string? EffectToolTip { get; set; }
        public int? EffectTypeId { get; set; }
        public int? EffectId { get; set; }
    }

    public class ReplacedHero
    {
        public required HeroCard HeroCard { get; set; }
        public required bool WasOnLeftSide { get; set; }
    }

    public class ReplacedHeroDisplay
    {
        public required HeroCardDisplay HeroCard { get; set; }
        public required bool WasOnLeftSide { get; set; }
    }

    public class ReplaceNextHeroEventData
    {
        public required HeroCard Hero { get; set; }
        public required AuraTypeWithLongevity ReplacementHeroAura { get; set; }
        public required Guid PlayerId { get; set; }
    }

    public class CurrentHeroCardData
    {
        public required bool LeftSide { get; set; }
        public required HeroCard HeroCard { get; set; }
        public required HeroCard UnUsedHeroCard { get; set; }
        public required int MovementFullLeft { get; set; }
        public required int MovementUnFullLeft { get; set; }
        public bool NoFractionMovement { get; set; } = false;
        public required List<int> VisitedPlaces { get; set; }
        public ReplacedHero? ReplacedHeroCard { get; set; }
    }

    public class CurrentHeroCard : INotifyPropertyChanged
    {
        public required bool LeftSide { get; set; }
        public required HeroCardDisplay HeroCard { get; set; }
        public required HeroCardDisplay UnUsedHeroCard { get; set; }
        public ReplacedHeroDisplay? ReplacedHeroCard { get; set; }
        private int _movementFullLeft;
        public int MovementFullLeft
        {
            get => _movementFullLeft;
            set
            {
                if (_movementFullLeft != value)
                {
                    _movementFullLeft = value;
                    OnPropertyChanged(nameof(MovementFullLeft));
                }
            }
        }

        private int _movementUnFullLeft;
        public int MovementUnFullLeft
        {
            get => _movementUnFullLeft;
            set
            {
                if (_movementUnFullLeft != value)
                {
                    _movementUnFullLeft = value;
                    OnPropertyChanged(nameof(MovementUnFullLeft));
                }
            }
        }

        private bool _noFactionMovement;
        public bool NoFractionMovement
        {
            get => _noFactionMovement;
            set
            {
                if (_noFactionMovement != value)
                {
                    _noFactionMovement = value;
                    OnPropertyChanged(nameof(NoFractionMovement));
                }
            }
        }

        private ObservableCollection<int> _visitedPlaces = new ObservableCollection<int>();
        public ObservableCollection<int> VisitedPlaces
        {
            get => _visitedPlaces;
            set
            {
                if (_visitedPlaces != value)
                {
                    if (_visitedPlaces != null)
                        _visitedPlaces.CollectionChanged -= OnVisitedPlacesChanged;

                    _visitedPlaces = value;
                    OnPropertyChanged(nameof(VisitedPlaces));

                    if (_visitedPlaces != null)
                        _visitedPlaces.CollectionChanged += OnVisitedPlacesChanged;
                }
            }
        }

        public CurrentHeroCard()
        {
            _visitedPlaces.CollectionChanged += OnVisitedPlacesChanged;
        }

        private void OnVisitedPlacesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(VisitedPlaces));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BuffHeroData
    {
        public required Guid PlayerId { get; set; }
        public required int HeroId { get; set; }
        public required Dictionary<ResourceHeroType, int> HeroResourcesNew { get; set; }
    }


    public class HeroCardDisplay : INotifyPropertyChanged
    {
        public required int Id { get; set; }
        public int? EffectId { get; set; }
        public required string HeroName { get; set; }
        private int _army;
        public int Army
        {
            get => _army;
            set
            {
                if (_army != value)
                {
                    _army = value;
                    OnPropertyChanged(nameof(Army));
                    OnPropertyChanged(nameof(HeroStatsParameter));
                }
            }
        }

        private int _magic;
        public int Magic
        {
            get => _magic;
            set
            {
                if (_magic != value)
                {
                    _magic = value;
                    OnPropertyChanged(nameof(Magic));
                    OnPropertyChanged(nameof(HeroStatsParameter));
                }
            }
        }

        private int _siege;
        public int Siege
        {
            get => _siege;
            set
            {
                if (_siege != value)
                {
                    _siege = value;
                    OnPropertyChanged(nameof(Siege));
                    OnPropertyChanged(nameof(HeroStatsParameter));
                }
            }
        }

        public required int Morale { get; set; }
        public required Fraction Faction { get; set; }
        public required int ScorePoints { get; set; }
        public required int RoyalSignet { get; set; }
        public required int MovementFull { get; set; }
        public required int MovementEmpty { get; set; }
        public required string ImageAtlas { get; set; }
        public DisplayInfo DisplayInfo { get; set; } = new DisplayInfo();
        public string? EffectImageAtlas { get; set; }
        public int? EffectImageIndex { get; set; }
        public string? EffectToolTip { get; set; }
        public int? EffectTypeId { get; set; }
        public EffectType? EffectType { get; set; }
        public EffectMercenaryType? EffectCard { get; set; }
        private int _imageIndex;
        public int ImageIndex
        {
            get => _imageIndex;
            set
            {
                _imageIndex = value;
                DisplayInfo = new DisplayInfo();
                DisplayInfo.SetupData(ImageAtlas, ImageIndex);
                if (EffectTypeId != null && EffectImageIndex != null && EffectImageAtlas != null)
                {
                    EffectType = new EffectType(EffectTypeId.Value);
                    EffectCard = new EffectMercenaryType(EffectImageAtlas, EffectImageIndex.Value);
                }
            }
        }

        public string HeroStatsParameter
        {
            get => "MovementFull,MovementEmpty,Siege,Magic,Army,IncomeGold,Morale,ScorePoints,RoyalSignet";
        }

        private bool _selected;
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



        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class HeroCardCombinedDisplay : INotifyPropertyChanged
    {
        public required int Id { get; set; }
        public required HeroCardDisplay LeftSide { get; set; }
        public required HeroCardDisplay RightSide { get; set; }
        public PlayerViewModel? _playerWhoPickedCard { get; set; }
        public PlayerViewModel? PlayerWhoPickedCard
        {
            get => _playerWhoPickedCard;
            set
            {
                if (_playerWhoPickedCard != value)
                {
                    _playerWhoPickedCard = value;
                    OnPropertyChanged(nameof(PlayerWhoPickedCard));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class HeroCardCombined
    {
        public PlayerViewModel? PlayerWhoPickedCard { get; set; }
        public required int Id { get; set; }
        public required HeroCard LeftSide { get; set; }
        public required HeroCard RightSide { get; set; }
    }

    public class HeroCardPicked
    {
        public required CurrentHeroCardData CurrentHeroCard { get; set; }
        public required HeroCard Card { get; set; }
        public required PlayerViewModel Player { get; set; }
        public Reward? Reward { get; set; }
    }


    public class Fraction
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                var FractionInfo = ResourcesFactionFromJson.GetById(_id)!;
                ImageIndex = FractionInfo.IconIndex;
                ImageAtlas = FractionInfo.IconAtlas;
                SetupFraction();
            }
        }
        public required string Name { get; set; }

        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public string ImageAtlas { get; set; } = "";
        public int ImageIndex { get; set; }
        public string ImagePathString { get; set; } = "";

        public Int32Rect CropRect => new Int32Rect(
            OffsetX,
            OffsetY,
            128,
            128
        );

        public void SetupFraction()
        {
            if (!string.IsNullOrEmpty(ImageAtlas))
            {
                var iconStruct = IconAtlases.GetIconAtlasByNameType(ImageAtlas).GetImageInfo(ImageIndex);

                OffsetX = iconStruct.OffsetX;
                OffsetY = iconStruct.OffsetY;
                ImagePathString = iconStruct.ImagePath;
            }
        }

        public string FractionIconPath
        {
            get
            {
                return Id switch
                {
                    1 => "pack://application:,,,/Assets/Fractions/Green.png",
                    2 => "pack://application:,,,/Assets/Fractions/Red.png",
                    3 => "pack://application:,,,/Assets/Fractions/Yellow.png",
                    4 => "pack://application:,,,/Assets/Fractions/Blue.png",
                    _ => "pack://application:,,,/Assets/Fractions/DefaultFraction.png",
                };
            }
        }
    }

    public class FractionInfo
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string IconAtlas { get; set; }
        public required int IconIndex { get; set; }
    }

    public class PlayerHeroData
    {
        public required List<HeroCard> LeftHeroCards { get; set; }
        public required List<HeroCard> RightHeroCards { get; set; }
        public required CurrentHeroCardData? CurrentHeroCard { get; set; }
    }

    public static class ResourcesFactionFromJson
    {

        public static readonly List<FractionInfo> ResourceFromJsonList;

        static ResourcesFactionFromJson()
        {
            string filePath = "Data/Faction.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                ResourceFromJsonList = JsonSerializer.Deserialize<List<FractionInfo>>(jsonData) ?? new List<FractionInfo>();
            }
            else
            {
                ResourceFromJsonList = new List<FractionInfo>();
            }
        }

        public static FractionInfo? GetById(int id)
        {
            return ResourceFromJsonList.FirstOrDefault(x => x.Id == id);
        }
    }
}