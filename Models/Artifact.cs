using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using BoardGameFrontend.VisualManager;

namespace BoardGameFrontend.Models
{
    public class Artifact
    {
        public required int Id { get; set; }
        public required string NameEng { get; set; }
        public required string NamePL { get; set; }
        public required string TextPL { get; set; }
        public required int ShuffleX { get; set; }
        public required string EffectIconAtlas { get; set; }
        public required int EffectIconIndex { get; set; }
        public required int Effect1 { get; set; }
        public required int Effect2 { get; set; }
        public int InGameIndex { get; set; }
        public required string BackgroundAtlas { get; set; }
        public required int BackgroundIndex { get; set; }
        public required int EffectType { get; set; }
        public required bool SecondEffectSuperior { get; set; }
    }

    public class EffectMercenaryType
    {
        public string BackgroundAtlas { get; set; }
        public int BackgroundIndex;
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string ImagePathString { get; set; } = "";
        public int IconWidth { get; set; } = 128;
        public int IconHeight { get; set; } = 128;

        public EffectMercenaryType(string backgroundAtlas, int backgroundIndex)
        {
            BackgroundAtlas = backgroundAtlas;
            BackgroundIndex = backgroundIndex;
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

    public class EffectType
    {
        public string BackgroundAtlas { get; set; }
        public int BackgroundIndex;
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string ImagePathString { get; set; } = "";
        public int IconWidth { get; set; } = 128;
        public int IconHeight { get; set; } = 128;

        public EffectType(int Id)
        {
            var effectType = EffectTypeFromJson.EffectsFromJsonList.Find(x => x.Id == Id);
            BackgroundAtlas = effectType.IconAtlas;
            BackgroundIndex = effectType.IconIndex;
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




    public class ArtifactPlayProperties : INotifyPropertyChanged
    {
        private bool _canPlayCard = true;
        private bool _canPlayEffectFirst = true;
        private bool _canPlayEffectSecond = false;
        private bool _thereIsSecondEffect = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanPlayCard
        {
            get => _canPlayCard;
            set
            {
                if (_canPlayCard != value)
                {
                    _canPlayCard = value;
                    OnPropertyChanged(nameof(CanPlayCard));
                }
            }
        }

        public bool CanPlayEffectFirst
        {
            get => _canPlayEffectFirst;
            set
            {
                if (_canPlayEffectFirst != value)
                {
                    _canPlayEffectFirst = value;
                    OnPropertyChanged(nameof(CanPlayEffectFirst));
                }
            }
        }

        public bool CanPlayEffectSecond
        {
            get => _canPlayEffectSecond;
            set
            {
                if (_canPlayEffectSecond != value)
                {
                    _canPlayEffectSecond = value;
                    OnPropertyChanged(nameof(CanPlayEffectSecond));
                }
            }
        }

        public bool ThereIsSecondEffect
        {
            get => _thereIsSecondEffect;
            set
            {
                if (_thereIsSecondEffect != value)
                {
                    _thereIsSecondEffect = value;
                    OnPropertyChanged(nameof(ThereIsSecondEffect));
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class ArtifactDisplay : INotifyPropertyChanged
    {
        public required int Id { get; set; }
        public ArtifactPlayProperties ArtifactPlayProperties { get; set; } = new ArtifactPlayProperties();
        public required string NameEng { get; set; }
        public required string NamePL { get; set; }
        public required string TextPL { get; set; }
        public required int ShuffleX { get; set; }
        public required string EffectIconAtlas { get; set; }
        public required int EffectIconIndex { get; set; }
        public required int Effect1 { get; set; }
        public required int Effect2 { get; set; }



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
        public required int EffectType { get; set; }
        public EffectMercenaryType EffectMercenaryType { get; set; }
        public EffectType EffectTypeDisplay { get; set; }
        public int InGameIndex { get; set; }
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

        public required bool SecondEffectSuperior { get; set; }
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
                EffectMercenaryType = new EffectMercenaryType(EffectIconAtlas, EffectIconIndex);
                EffectTypeDisplay = new EffectType(EffectType);
            }
        }

        public Int32Rect CropRect => new Int32Rect(
            OffsetX,
            OffsetY,
            IconWidth,
            IconHeight
        );

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public class ArtifactToPickFromData
    {
        public required int ArtifactsLeft { get; set; }
        public required int ArtifactsLeftTossed { get; set; }
        public required List<Artifact> Artifacts { get; set; }
        public required PlayerViewModel Player { get; set; }
    }

    public class ArtifactToPickFromDataForOtherUsers
    {
        public required int ArtifactsLeft { get; set; }
        public required int ArtifactsLeftTossed { get; set; }
        public required int ArtifactsAmount { get; set; }
        public required PlayerViewModel Player { get; set; }
    }

    public class ArtifactTakenData
    {
        public required int ArtifactsLeft { get; set; }
        public required int ArtifactsLeftTossed { get; set; }
        public required List<Artifact> Artifacts { get; set; }
        public required PlayerViewModel Player { get; set; }
    }

    public class ArtifactTakenDataForOtherUsers
    {
        public required int ArtifactsLeft { get; set; }
        public required int ArtifactsLeftTossed { get; set; }
        public required int ArtifactsAmount { get; set; }
        public required PlayerViewModel Player { get; set; }
    }

    public class LockedByPlayerInfo
    {
        public required Guid PlayerId { get; set; }
        public required string PlayerName { get; set; }
    }

    public class ArtifactPlayed
    {
        public required Artifact Artifact { get; set; }
        public required PlayerViewModel Player { get; set; }
        public required bool FirstEffect { get; set; }
        public required Reward Reward { get; set; }
    }

    public static class EffectTypeFromJson
    {

        public static readonly List<EffectTypeJson> EffectsFromJsonList;

        static EffectTypeFromJson()
        {
            string filePath = "Data/EffectType.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                EffectsFromJsonList = JsonSerializer.Deserialize<List<EffectTypeJson>>(jsonData) ?? new List<EffectTypeJson>();
            }
            else
            {
                EffectsFromJsonList = new List<EffectTypeJson>();
            }
        }
    }

    public class EffectTypeJson
    {
        public required int Id { get; set; }
        public required string NameType { get; set; }
        public required string IconAtlas { get; set; }
        public required int IconIndex { get; set; }
    }

    public class ArtifactRerolledData
    {
        public required Artifact Artifact { get; set; }
        public required PlayerViewModel Player { get; set; }
        public required Artifact ArtifactRerolled { get; set; }
    }

    public class ArtifactRerolledDataForOtherUsers
    {
        public required PlayerViewModel Player { get; set; }
    }

    public class ArtifactInfo
    {
        public required List<Artifact> ArtifactToPickFrom {get; set;}
        public required int ArtifactsLeftAmount { get; set; }
        public required int ArtifactsTossedAwayAmount { get; set; }
    }

    public class ArtifactPhaseSkipped
    {
        public required Guid PlayerId { get; set; }
    }

    public class ArtifactPlayerData
    {
        public required List<Artifact> ArtifactsPlayed { get; set; }
        public required List<Artifact> ArtifactsOwned { get; set; }
    }
}