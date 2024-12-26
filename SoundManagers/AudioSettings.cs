using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BoardGameFrontend.Helpers
{
    public class AudioSettingsData
    {
        public double Volume { get; set; }
        public bool IsMusicEnabled { get; set; }
        public string SelectedTrack { get; set; }
        public bool ShuffleEnabled {get; set;}
    }

    public static class AudioSettings
    {
        private static double _volume = 0.3;
        private static bool _isMusicEnabled = true;
        private static string _selectedTrack;

        private static bool _shuffleEnabled = true;
        public static bool ShuffleEnabled
        {
            get => _shuffleEnabled;
            set
            {
                if (_shuffleEnabled != value)
                {
                    _shuffleEnabled = value;
                    ShuffleStatusChanged?.Invoke(_shuffleEnabled);
                }
            }
        }

        public static bool WasSaved { get; private set; }

        private static readonly string SettingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AudioSettings.json");
        public static List<string> AvailableTracks { get; set; } = new List<string>();

        public static double Volume
        {
            get => _volume;
            set
            {
                _volume = Math.Clamp(value, 0.0, 1.0);
                VolumeChanged?.Invoke(_volume);
            }
        }

        public static bool IsMusicEnabled
        {
            get => _isMusicEnabled;
            set
            {
                _isMusicEnabled = value;
                MusicStatusChanged?.Invoke(_isMusicEnabled);
            }
        }

        public static string SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                if (_selectedTrack != value)
                {
                    _selectedTrack = value;
                    SelectedTrackChanged?.Invoke(_selectedTrack);
                }
            }
        }

        public static event Action<double> VolumeChanged;
        public static event Action<bool> MusicStatusChanged;
        public static event Action<string> SelectedTrackChanged;
        public static event Action<bool> ShuffleStatusChanged;

        public static void Initialize(List<string> availableTracks)
        {
            AvailableTracks = availableTracks;

            LoadSettings();

            if (AvailableTracks.Count > 0 && string.IsNullOrEmpty(SelectedTrack))
            {
                SelectedTrack = AvailableTracks[0];
            }
        }

        public static void SaveSettings()
        {
            WasSaved = true;
            var settingsData = new AudioSettingsData
            {
                Volume = Volume,
                IsMusicEnabled = IsMusicEnabled,
                SelectedTrack = SelectedTrack,
                ShuffleEnabled = ShuffleEnabled
            };

            var json = JsonConvert.SerializeObject(settingsData, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
        }

        public static void LoadSettings()
        {
            WasSaved = false;
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                var settingsData = JsonConvert.DeserializeObject<AudioSettingsData>(json);

                if (settingsData != null)
                {
                    Volume = settingsData.Volume;
                    IsMusicEnabled = settingsData.IsMusicEnabled;
                    SelectedTrack = settingsData.SelectedTrack;
                    ShuffleEnabled = settingsData.ShuffleEnabled;
                }
            }
        }
    }
}
