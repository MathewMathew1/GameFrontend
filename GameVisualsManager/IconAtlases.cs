using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using BoardGameFrontend.Models;
using System;
using System.Linq;

namespace BoardGameFrontend.VisualManager
{


    public static class IconAtlases
    {
    
        public static List<IconAtlas> Atlases { get; set; } = new List<IconAtlas>();
        

        static IconAtlases()
        {
            var AtlasesList = LoadIconsFromFile("Data/Atlases.json");
            Atlases.Clear(); 
            foreach (var atlas in AtlasesList)
            {
                Atlases.Add(atlas); 
            }
        }

        public static IconAtlas GetIconAtlasByNameType(string nameType){
            var matchingAtlas = Atlases.FirstOrDefault(x => x.NameType == nameType);

            return matchingAtlas ?? Atlases.FirstOrDefault()!;
        }

        private static List<IconAtlas> LoadIconsFromFile(string filePath)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"The file '{filePath}' was not found at path: {fullPath}");
            }

            var jsonData = File.ReadAllText(fullPath);
            var atlasList = JsonConvert.DeserializeObject<List<IconAtlas>>(jsonData)!;

            return atlasList;
        }

    }
}