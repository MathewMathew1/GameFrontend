
using System.ComponentModel;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    public class MiniPhaseManager : INotifyPropertyChanged
    {
        private MiniPhase? _currentPhase = null;
        public MiniPhase? CurrentPhase
        {
            get => _currentPhase;
            set
            {
                _currentPhase = value;
                OnPropertyChanged(nameof(CurrentPhase));
            }
        }

        private Game _game { get; set; }
        

        public MiniPhaseManager(Game game)
        {
            _game = game;
        }

        public void EndPhase(){
            CurrentPhase = null;
        }

        public void SetCurrentPhase(MiniPhase? phase){
            CurrentPhase = phase;

            if(phase?.Name == MiniPhaseType.BlockTilePhase){
                _game.GameVisualManager.TilesBorderManager.SetBlockTilesConnections();
            }
        }

        public bool IsCurrentPhase(MiniPhase phase) => phase == CurrentPhase;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}