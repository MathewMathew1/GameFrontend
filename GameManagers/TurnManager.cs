using System.ComponentModel;

namespace BoardGameFrontend.Managers
{
    public class TurnManager : INotifyPropertyChanged
    {
        private int _turnCount;
        public int TurnCount
        {
            get => _turnCount;
            set
            {
                _turnCount = value;
                OnPropertyChanged(nameof(TurnCount));
            }
        }

        private int _roundCount;
        public int RoundCount
        {
            get => _roundCount;
            set
            {
                _roundCount = value;
                OnPropertyChanged(nameof(RoundCount));
            }
        }

        private int _currentPlayerIndex;

        public int CurrentPlayerIndex
        {
            get => _currentPlayerIndex;
            set
            {
                _currentPlayerIndex = value;
                OnPropertyChanged(nameof(CurrentPlayerIndex));
            }
        }

 
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}