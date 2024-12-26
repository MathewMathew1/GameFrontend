using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using BoardGameFrontend.VisualManager;

namespace BoardGameFrontend.Models
{
    public enum ResourceType
    {
        Gold,
        Wood,
        Iron,
        Gems,
        Niter,
        MysticFog
    }

    public class ResourceReceivedEventData
    {
        public required List<Resource> Resources { get; set; }
        public required string ResourceInfo { get; set; }
        public required Guid PlayerId {get; set;}
    }


    public class ResourceInfo : INotifyPropertyChanged
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
        public string ToolTipName { get; set; }
        public string ImageAtlas { get; set; }
        public int ImageIndex { get; set; }
        public string ImagePathString { get; set; } = "";
        public int IconWidth  { get; set; } = 128;
        public int IconHeight  { get; set; } = 128;

        public Int32Rect CropRect => new Int32Rect(
            OffsetX,
            OffsetY,
            IconHeight , 
            IconWidth 
        );

        public ResourceInfo(int amount, ResourceFromJson resourceFromJson)
        {
            _amount = amount;
            ImageIndex = resourceFromJson.ImageIndex;
            ImageAtlas = resourceFromJson.ImageAtlas;
                
            ToolTipName = resourceFromJson.NamePL;
            SetupTile();
        }
        

        public void SetupTile()
        {
            if (!string.IsNullOrEmpty(ImageAtlas))
            {
                var iconStruct = IconAtlases.GetIconAtlasByNameType(ImageAtlas).GetImageInfo(ImageIndex);
                IconHeight = iconStruct.IconHeight;
                IconWidth = iconStruct.IconWidth;
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

    public class ResourceManagerViewModel
    {
        public required Dictionary<ResourceType, int> Resources { get; set; }
    }

    public class ResourceFromJson
    {
        public required string NameEng { get; set; }
        public required string NamePL { get; set; }
        public required string ImageAtlas { get; set; }
        public required int Id { get; set; }
        public required int ImageIndex { get; set; }
        public required bool ResetsOnTurn { get; set; }
    }

    public static class ResourcesFromJson
    {

        public static readonly List<ResourceFromJson> ResourceFromJsonList;

        static ResourcesFromJson()
        {
            string filePath = "Data/Resource.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                ResourceFromJsonList = JsonSerializer.Deserialize<List<ResourceFromJson>>(jsonData) ?? new List<ResourceFromJson>();
            }
            else
            {
                ResourceFromJsonList = new List<ResourceFromJson>();
            }
        }
    }
}