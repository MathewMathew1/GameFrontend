using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using BoardGameFrontend.Helpers;

namespace BoardGameFrontend.Managers
{
    public class TokenManager : INotifyPropertyChanged
    {

        private ObservableCollection<int> _selectedTokensId= new ObservableCollection<int>();
        public ObservableCollection<int> SelectedTokensId
        {
            get => _selectedTokensId;
            set
            {
                if (_selectedTokensId != value)
                {
                    if (_selectedTokensId != null)
                        _selectedTokensId.CollectionChanged -= OnVisitedPlacesChanged;

                    _selectedTokensId = value;
                    OnPropertyChanged(nameof(SelectedTokensId));

                    if (_selectedTokensId != null)
                        _selectedTokensId.CollectionChanged += OnVisitedPlacesChanged;
                }
            }
        }

        private void OnVisitedPlacesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedTokensId));
        }

        private bool? _correctNumberOfSelectedIds;
        public bool? CorrectNumberOfSelectedIds
        {
            get { return _correctNumberOfSelectedIds; }
            set
            {
                if (_correctNumberOfSelectedIds != value)
                {
                    _correctNumberOfSelectedIds = value;
                    OnPropertyChanged(nameof(CorrectNumberOfSelectedIds));
                }
            }
        }
        private Game _game { get; }

        public TokenManager(Game game)
        {
            _game = game;
            _selectedTokensId.CollectionChanged += OnVisitedPlacesChanged;
        }

        public void ClearSelection(){
            SelectedTokensId.Clear();
        }

        public void SelectTokenByTileId(int id)
        {
            var tile = _game.GameVisualManager.GameTiles.GetTileById(id);
            if (tile == null) return;

            if (tile.Token == null || tile.TileType.Id != TileHelper.CastleTileId) return;

            if (SelectedTokensId.Contains(id))
            {
                SelectedTokensId.Remove(id);
            }
            else
            {
                SelectedTokensId.Add(id);
            }

            CorrectNumberOfSelectedIds = SelectedTokensId.Count == 2;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}