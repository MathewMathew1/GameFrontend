using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Media;

    public class LobbyViewModelManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly List<Color> PlayerColors = new List<Color> {  Color.FromRgb(0, 121, 11), Color.FromRgb(153, 102, 255), Colors.Blue, Colors.Red, Color.FromRgb(255, 132, 0) };
        private readonly Dictionary<Guid, Color> _playerColorMapping = new();

        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                    OnPropertyChanged(nameof(CanDestroyLobby));
                    OnPropertyChanged(nameof(CanStartGame));
                }
            }
        }

        private Guid _hostId;
        public Guid HostId
        {
            get => _hostId;
            set
            {
                if (_hostId != value)
                {
                    _hostId = value;
                    OnPropertyChanged(nameof(HostId));
                    OnPropertyChanged(nameof(CanDestroyLobby));
                    OnPropertyChanged(nameof(CanStartGame));
                }
            }
        }

        private string _lobbyName;
        public string LobbyName
        {
            get => _lobbyName;
            set
            {
                if (_lobbyName != value)
                {
                    _lobbyName = value;
                    OnPropertyChanged(nameof(LobbyName));
                }
            }
        }

        private ObservableCollection<PlayerViewModel> _players;
        public ObservableCollection<PlayerViewModel> Players
        {
            get => _players;
            set
            {
                if (_players != value)
                {
                    _players = value;
                    OnPropertyChanged(nameof(Players));
                }
            }
        }

        private bool _gameHaveStarted;
        public bool GameHaveStarted
        {
            get => _gameHaveStarted;
            set
            {
                if (_gameHaveStarted != value)
                {
                    _gameHaveStarted = value;
                    OnPropertyChanged(nameof(GameHaveStarted));
                    OnPropertyChanged(nameof(CanStartGame));
                }
            }
        }

        private Guid _currentUserId;
        public Guid CurrentUserId
        {
            get => _currentUserId;
            set
            {
                if (_currentUserId != value)
                {
                    _currentUserId = value;
                    OnPropertyChanged(nameof(CurrentUserId));
                    OnPropertyChanged(nameof(CanDestroyLobby));
                    OnPropertyChanged(nameof(CanStartGame));
                }
            }
        }

        public bool CanDestroyLobby => HostId == CurrentUserId;

        public bool CanStartGame => HostId == CurrentUserId && !GameHaveStarted;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LobbyViewModelManager(LobbyDetailedViewModel lobby)
        {
            _id = lobby.Id;
            _hostId = lobby.HostId;
            _lobbyName = lobby.LobbyName;
            _players = new ObservableCollection<PlayerViewModel>(lobby.Players);
            _currentUserId = new Guid();
        }

        public void AddPlayer(PlayerViewModel player)
        {
            Players.Add(player);
            AssignColorToPlayer(player.Id);
        }

        public void RemovePlayerById(Guid playerId)
        {
            var playerToRemove = Players.FirstOrDefault(p => p.Id == playerId);
            if (playerToRemove != null)
            {
                Players.Remove(playerToRemove);
                _playerColorMapping.Remove(playerId);
            }
        }

        public Color GetColorForPlayer(Guid playerId)
        {
            return _playerColorMapping.TryGetValue(playerId, out var color) ? color : Colors.Blue;
        }

        private void AssignColorToPlayer(Guid playerId)
        {
            var colorIndex = _playerColorMapping.Count % PlayerColors.Count;
            var colorToAssign = PlayerColors[colorIndex];
            _playerColorMapping[playerId] = colorToAssign;
        }

        public void UpdateLobby(LobbyDetailedViewModel lobbyData)
        {
            Id = lobbyData.Id;
            HostId = lobbyData.HostId;
            LobbyName = lobbyData.LobbyName;

            Players.Clear();
            _playerColorMapping.Clear();
            foreach (var player in lobbyData.Players)
            {
                AddPlayer(player);
            }
        }
    }

}