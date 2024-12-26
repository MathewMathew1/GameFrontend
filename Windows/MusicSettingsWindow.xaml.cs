using System.Windows;
using BoardGameFrontend.Helpers;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Windows;

namespace BoardGameFrontend
{
    public partial class MusicSettingsWindow : FullScreenWindow
    {
        // Fields to store initial values
        private readonly double _initialVolume;
        private readonly bool _initialIsMusicEnabled;
        private readonly string _initialSelectedTrack;

        public MusicSettingsWindow()
        {
            InitializeComponent();
            
            _initialVolume = AudioSettings.Volume;
            _initialIsMusicEnabled = AudioSettings.IsMusicEnabled;
            _initialSelectedTrack = AudioSettings.SelectedTrack;

            DataContext = new SoundOptionsViewModel();

            this.Closing += MusicSettingsWindow_Closing;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RestoreInitialSettings();
            Close();
            Owner?.Show(); // Show MainWindow again
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            AudioSettings.SaveSettings(); // Save the current settings to file
            Close();
            Owner?.Show();
        }

        // This method will be called when the window is closed by any method
        private void MusicSettingsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Restore initial settings only if the Save button was not clicked
            if (!AudioSettings.WasSaved)
            {
                RestoreInitialSettings();
            }
        }

        // Restore the initial values to AudioSettings
        private void RestoreInitialSettings()
        {
            AudioSettings.Volume = _initialVolume;
            AudioSettings.IsMusicEnabled = _initialIsMusicEnabled;
            AudioSettings.SelectedTrack = _initialSelectedTrack;
        }
    }
}
