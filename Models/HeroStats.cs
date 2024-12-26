
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using BoardGameFrontend.VisualManager;

namespace BoardGameFrontend.Models
{
    public class HeroStats
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int IconIndex { get; set; }

        public required string IconAtlas { get; set; }
    }

    public class HeroStatsWithEngName
    {
        public required int Id { get; set; }
        public required string NameEng { get; set; }
        public required int ImageIndex { get; set; }

        public required string ImageAtlas { get; set; }
    }

    public class HeroStat
    {
        public required string Name { get; set; }
        public int Value { get; set; } 
        public required string ImageKey { get; set; } 
    }

    public class HeroStatsWithAtlas
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IconIndex { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string IconAtlas { get; set; }
        public string ImagePathString { get; set; } = "";
        public int IconWidth { get; set; } = 128;
        public int IconHeight { get; set; } = 128;

        public HeroStatsWithAtlas(HeroStats heroStats)
        {
            Id = heroStats.Id;
            Name = heroStats.Name;
            IconIndex = heroStats.IconIndex;
            IconAtlas = heroStats.IconAtlas;
            SetupTile();
        }

        public HeroStatsWithAtlas(HeroStatsWithEngName heroStats)
        {
            Id = heroStats.Id;
            Name = heroStats.NameEng;
            IconIndex = heroStats.ImageIndex;
            IconAtlas = heroStats.ImageAtlas;
            SetupTile();
        }

        public HeroStatsWithAtlas(MercenaryTypeCard mercenaryData)
        {
            Id = mercenaryData.Id;
            Name = mercenaryData.NameType;
            IconIndex = mercenaryData.IconIndex;
            IconAtlas = mercenaryData.IconAtlas;
            SetupTile();
        }

        public void SetupTile()
        {
            if (!string.IsNullOrEmpty(IconAtlas))
            {
                var iconStruct = IconAtlases.GetIconAtlasByNameType(IconAtlas).GetImageInfo(IconIndex);

                OffsetX = iconStruct.OffsetX;
                OffsetY = iconStruct.OffsetY;
                IconHeight = iconStruct.IconHeight;
                IconWidth = iconStruct.IconWidth;
                ImagePathString = iconStruct.ImagePath;
            }
        }

         

        public Int32Rect CropRect => new Int32Rect(
            OffsetX,
            OffsetY,
            IconHeight,
            IconWidth
        );

    }

    public static class HeroStatsFactory
    {

        public static readonly List<HeroStats> ResourceFromJsonList = new List<HeroStats>();
        public static readonly List<HeroStatsWithEngName> ResourceFromJsonListWithEng = new List<HeroStatsWithEngName>();
        public static readonly List<HeroStatsWithAtlas> HeroStatsWithAtlas = new List<HeroStatsWithAtlas>();

        static HeroStatsFactory()
        {
            string filePath = "Data/ResourceHero.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                ResourceFromJsonList = JsonSerializer.Deserialize<List<HeroStats>>(jsonData) ?? new List<HeroStats>();
                ResourceFromJsonList.ForEach(resource =>
                {
                    HeroStatsWithAtlas.Add(new HeroStatsWithAtlas(resource));
                });
            }

            filePath = "Data/OthersStats.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                ResourceFromJsonList = JsonSerializer.Deserialize<List<HeroStats>>(jsonData) ?? new List<HeroStats>();
                ResourceFromJsonList.ForEach(resource =>
                {
                    HeroStatsWithAtlas.Add(new HeroStatsWithAtlas(resource));
                });
            }

            filePath = "Data/Resource.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                ResourceFromJsonListWithEng = JsonSerializer.Deserialize<List<HeroStatsWithEngName>>(jsonData) ?? new List<HeroStatsWithEngName>();
                ResourceFromJsonListWithEng.ForEach(resource =>
                {
                    HeroStatsWithAtlas.Add(new HeroStatsWithAtlas(resource));
                });
            }

 

        }

        public static HeroStatsWithAtlas GetByName(string name)
        {
            return HeroStatsWithAtlas.FirstOrDefault(x => x.Name == name) ;
        }
    }
}