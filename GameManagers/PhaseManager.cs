using System.Collections.ObjectModel;
using System.ComponentModel;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    public class PhaseManager : INotifyPropertyChanged
    {
        private ObservableCollection<Phase> _phases;
        private Phase _currentPhase;
        private Game _game;
        public bool PhaseJustStarted = false;

        public PhaseManager(Game game)
        {
            _game = game;
            _phases = new ObservableCollection<Phase>
            {
                new Phase { Name = PhaseType.ArtifactPhase},
                new Phase { Name = PhaseType.HeroCardPickingPhase },
                new Phase { Name = PhaseType.BoardPhase },
                new Phase { Name = PhaseType.MercenaryPhase }
                // Add other phases as needed
            };

            // Set the current phase (this could be dynamically set based on game state)
            _currentPhase = _phases[0];
        }

        public ObservableCollection<Phase> Phases
        {
            get => _phases;
            set
            {
                _phases = value;
                OnPropertyChanged(nameof(Phases));
            }
        }

        public Phase CurrentPhase
        {
            get => _currentPhase;
            set
            {
                _currentPhase = value;
                OnPropertyChanged(nameof(CurrentPhase));
            }
        }

        public void SetCurrentPhase(Phase phase){
            CurrentPhase = phase;
            if(false){
                _game.PlayerManager.ResetAllPlayersPlayedTurn();
            }
            PhaseJustStarted = true;
            if(phase.Name==PhaseType.BoardPhase && _game.CurrentPlayer.PlayerHeroCardManager.CurrentHeroCard != null){
                _game.CurrentPlayer.PlayerHeroCardManager.CurrentHeroCard.VisitedPlaces.Add(_game.PawnManager.CurrentTile.Id);
            }
        }

        public bool IsCurrentPhase(Phase phase) => phase == CurrentPhase;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}