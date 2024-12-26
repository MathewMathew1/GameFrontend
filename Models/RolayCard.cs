using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BoardGameFrontend.Models
{
    public class RolayCard
    {
        public required string Type { get; set; }
        public required int Id { get; set; }
        public required Fraction Faction { get; set; }
        public required int ScorePoints { get; set; }
        public required int Army { get; set; }
        public required int Siege { get; set; }
        public required int Magic { get; set; }
        public required int Morale { get; set; }
        public string? ImageAtlas { get; set; }
        public int? ImageIndex { get; set; }
        public int? EffectId { get; set; }
        public int? EffectImageIndex { get; set; }
        public string? EffectImageAtlas { get; set; }
        public int? EffectTypeId { get; set; }
        public string? EffectToolTip { get; set; }
        public PlayerViewModel? PickedByPlayer { get; set; }
        public required bool BanishedCard {get; set;}
    }

    public class RolayCardDisplay : INotifyPropertyChanged
    {
        public required string Type { get; set; }
        public required int Id { get; set; }
        public required Fraction Faction { get; set; }
        public required int ScorePoints { get; set; }
        public required int Army { get; set; }
        public required int Siege { get; set; }
        public required int Magic { get; set; }
        public required int Morale { get; set; }
        public string? ImageAtlas { get; set; }

        private bool _banishedCard;
        public bool BanishedCard
        {
            get => _banishedCard;
            set
            {
                _banishedCard = value;
                OnPropertyChanged(nameof(BanishedCard));
            }
        }

        public int? EffectId { get; set; }
        public int? EffectImageIndex { get; set; }
        public string? EffectImageAtlas { get; set; }
        public int? EffectTypeId { get; set; }
        public string? EffectToolTip { get; set; }
        private PlayerViewModel? _pickedByPlayer;
        public PlayerViewModel? PickedByPlayer
        {
            get => _pickedByPlayer;
            set
            {
                if (_pickedByPlayer != value)
                {
                    _pickedByPlayer = value;
                    OnPropertyChanged(nameof(PickedByPlayer));
                }
            }
        }

        public DisplayInfo DisplayInfo { get; set; } = new DisplayInfo();
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RoyalCardPlayed
    {
        public required RolayCard RoyalCard { get; set; }
        public required Reward? Reward { get; set; }
        public required Guid PlayerId { get; set; }
        public required int AmountOfSignetsForNextRoyalCard {get; set;}
    }

    public class RoyalCardsPlayerData{
        public required List<RolayCard> RoyalCards{ get; set; }
        public required int SignetsNeededForNextCard{ get; set; }
    }

    public class BanishRoyalCardEventData
    {
        public required RolayCard RoyalCard { get; set; }
        public required Guid PlayerId {get; set;}
    }
}