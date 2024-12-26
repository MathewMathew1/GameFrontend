using System.Collections.Generic;
using System.ComponentModel;

namespace BoardGameFrontend.Models
{
    public class Phase : INotifyPropertyChanged
    {
        private PhaseType _name;
        public PhaseType Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum PhaseType
    {
        HeroCardPickingPhase,
        BoardPhase,
        MercenaryPhase,
        DummyPhase,
        ArtifactPhase
        
        // Add other phases as needed
    }

    public class DummyPhaseStarted
    {
        public required PlayerViewModel Player { get; set; }
    }

    public class EndOfPlayerTurnData
    {
        public required PlayerViewModel Player { get; set; }
    }

    public class HeroTurnEnded
    {
        public required PlayerViewModel Player { get; set; }
    }

    public class StartTurnData
    {
        public required PlayerViewModel Player { get; set; }
        public required int TurnCount { get; set; }
        public required List<HeroCardCombined> NewCards { get; set; }
        public required int RoundCount { get; set; }
    }

}