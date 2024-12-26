using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BoardGameFrontend.Helpers;

namespace BoardGameFrontend.Managers
{
    public class SoundOptionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public SoundOptionsViewModel()
        {
            AvailableTracks = AllMusicsManager.GetAvailableTracks();
            SelectedTrack = AudioSettings.SelectedTrack;
            AudioSettings.SelectedTrackChanged += OnSelectedTrackChanged; // Subscribe to track changes
        }

        public double Volume
        {
            get => AudioSettings.Volume;
            set
            {
                if (AudioSettings.Volume != value)
                {
                    AudioSettings.Volume = value;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        public bool IsMusicEnabled
        {
            get => AudioSettings.IsMusicEnabled;
            set
            {
                if (AudioSettings.IsMusicEnabled != value)
                {
                    AudioSettings.IsMusicEnabled = value;
                    OnPropertyChanged(nameof(IsMusicEnabled));
                }
            }
        }

        public bool ShuffleEnabled
        {
            get => AudioSettings.ShuffleEnabled;
            set
            {
                if (AudioSettings.ShuffleEnabled != value)
                {
                    AudioSettings.ShuffleEnabled = value;
                    OnPropertyChanged(nameof(ShuffleEnabled));
                }
            }
        }

        public List<string> AvailableTracks { get; }

        public string SelectedTrack
        {
            get => AudioSettings.SelectedTrack;
            set
            {
                if (AudioSettings.SelectedTrack != value)
                {
                    AudioSettings.SelectedTrack = value;
                    OnPropertyChanged(nameof(SelectedTrack));
                }
            }
        }

        public string SelectedTrackPath => AllMusicsManager.GetTrackPath(SelectedTrack);

        private void OnSelectedTrackChanged(string newTrack)
        {
            OnPropertyChanged(nameof(SelectedTrack));
            OnPropertyChanged(nameof(SelectedTrackPath)); // Notify that the track path has changed
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
