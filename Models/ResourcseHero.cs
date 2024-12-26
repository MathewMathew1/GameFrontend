using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using BoardGameFrontend.VisualManager;

namespace BoardGameFrontend.Models
{
    public enum ResourceHeroType
    {
        Siege,
        Army,
        Magic,
        Signet
    }

    public class ResourceHeroInfo : INotifyPropertyChanged
    {
        private int _amount;
        public int Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    OnPropertyChanged(nameof(Amount));
                }
            }
        }

        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public string ImageAtlas { get; set; }
        public int ImageIndex { get; set; }
        public string ImagePathString { get; set; } = "";

        public Int32Rect CropRect => new Int32Rect(
            OffsetX,
            OffsetY,
            128,
            128
        );

        public ResourceHeroInfo(int amount, ResourceHeroFromJson resourceHeroFromJson)
        {
            _amount = amount;
            ImageIndex = resourceHeroFromJson.IconIndex;
            ImageAtlas = resourceHeroFromJson.IconAtlas;
            SetupTile();
        }

        public void SetupTile()
        {
            if (!string.IsNullOrEmpty(ImageAtlas))
            {
                var iconStruct = IconAtlases.GetIconAtlasByNameType(ImageAtlas).GetImageInfo(ImageIndex);

                OffsetX = iconStruct.OffsetX;
                OffsetY = iconStruct.OffsetY;
                ImagePathString = iconStruct.ImagePath;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ResourceHeroFromJson
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string IconAtlas { get; set; }
        public required int IconIndex { get; set; }
    }

    public static class ResourcesHeroFromJson
    {

        public static readonly List<ResourceHeroFromJson> ResourceFromJsonList;

        static ResourcesHeroFromJson()
        {
            string filePath = "Data/ResourceHero.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                ResourceFromJsonList = JsonSerializer.Deserialize<List<ResourceHeroFromJson>>(jsonData) ?? new List<ResourceHeroFromJson>();
            }
            else
            {
                ResourceFromJsonList = new List<ResourceHeroFromJson>();
            }
        }
    }

    public class HeroResource
    {
        public ResourceHeroType Type { get; set; }
        public int Amount { get; set; }

        public HeroResource(ResourceHeroType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }

}