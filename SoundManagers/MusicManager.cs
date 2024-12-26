using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BoardGameFrontend.Helpers
{
    public static class AllMusicsManager
    {
        private static readonly string MusicFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");

        public static List<string> GetAvailableTracks()
        {
            if (!Directory.Exists(MusicFolder))
                return new List<string>();

            var tracks = Directory.GetFiles(MusicFolder, "*.mp3")
                            .Select(Path.GetFileNameWithoutExtension)
                            .ToList();
             
            return tracks;
        }

        public static string GetTrackPath(string trackName)
        {
            return Path.Combine(MusicFolder, trackName + ".mp3");
        }
    }
}