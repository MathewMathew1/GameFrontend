using System.Windows;
using BoardGameFrontend.Models;
using BoardGameFrontend.Windows;

namespace BoardGameFrontend
{
    public partial class GameOptionsWindow : FullScreenWindow
    {
        public GameOptions GameOptions;

        public GameOptionsWindow()
        {
            GameOptions = GameOptions.LoadFromFile();
            DataContext = GameOptions; 

            InitializeComponent(); 
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            GameOptions.SaveToFile();
            this.Close(); 
            Owner?.Show(); 
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Owner?.Show(); // Show MainWindow again
        }
    }
}