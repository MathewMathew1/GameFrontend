
using System.ComponentModel;
using BoardGameFrontend.Managers;

namespace BoardGameFrontend.VisualManager
{


    public class GameVisualManager: INotifyPropertyChanged
    {
        public  GameTiles GameTiles { get; set; } = new GameTiles();
        public  TileBorderManager TilesBorderManager { get; set; }

        public GameVisualManager(Game game)
        {   
            TilesBorderManager = new TileBorderManager(game);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
}