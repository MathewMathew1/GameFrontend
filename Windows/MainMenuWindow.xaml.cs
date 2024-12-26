using System.Windows;
using BoardGameFrontend.Windows;

namespace BoardGameFrontend
{
    public partial class MainMenuWindow : FullScreenWindow
    {
        public MainMenuWindow()
        {
            InitializeComponent();
        }

        private void NavigateToInstructions(object sender, RoutedEventArgs e)
        {
            var instructionsWindow = new InstructionsWindow();
            instructionsWindow.Owner = this; // Set MainWindow as owner
            instructionsWindow.Show();
            this.Hide();
        }

        private void NavigateToOptions(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new GameOptionsWindow();
            optionsWindow.Owner = this;
            optionsWindow.Show();
            this.Hide();
        }

        private void NavigateToMusic(object sender, RoutedEventArgs e)
        {
            var musicWindow = new MusicSettingsWindow();
            musicWindow.Owner = this;
            musicWindow.Show();
            this.Hide();
        }
    }
}