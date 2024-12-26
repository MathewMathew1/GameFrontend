using System.ComponentModel;
using BoardGameFrontend.Models;
using System.Collections.ObjectModel;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Helpers;

namespace BoardGameFrontend.VisualManager
{
    public class TileBorderManager : INotifyPropertyChanged
    {
        private Game _game;

        public TileBorderManager(Game game)
        {
            _game = game;
        }

        private ObservableCollection<int> _tilesWithBorder = new ObservableCollection<int>();
        public ObservableCollection<int> TilesWithBorder
        {
            get => _tilesWithBorder;
            set
            {
                if (_tilesWithBorder != value)
                {
                    _tilesWithBorder = value;
                    OnPropertyChanged(nameof(TilesWithBorder));
                }
            }
        }

        public void SetAvailableConnections()
        {
            if (_game.MiniPhaseManager.CurrentPhase?.Name == MiniPhaseType.TeleportationPhase)
            {
                SetTeleportConnections();
                return;
            }

            var CurrentTile = _game.PawnManager.CurrentTile;
            var TeleportationIsOn = _game.PawnManager.TeleportationIsOn;
            var player = _game.UserControlledPlayer;
            var currentHeroCard = player.PlayerHeroCardManager.CurrentHeroCard;

            if (currentHeroCard == null) return;

            if (currentHeroCard.MovementUnFullLeft == 0 && currentHeroCard.MovementFullLeft == 0)
            {
                return;
            }

            var newTilesWithBorder = new ObservableCollection<int>();

            if (TeleportationIsOn)
            {
                var tiles = _game.GameVisualManager.GameTiles.Tiles;

                foreach (var tile in tiles)
                {
                    if (tile.TileType.Id == TileHelper.MagicTileId && tile.Id != CurrentTile.Id)
                    {
                        ;
                        if (tile.IsInRangeOfCastle(currentHeroCard.HeroCard.Faction, 3)) continue;
                        newTilesWithBorder.Add(tile.Id);
                    }
                }

                TilesWithBorder = newTilesWithBorder;
                return;
            }

            foreach (var connection in CurrentTile.Connections)
            {
                if (currentHeroCard.VisitedPlaces.Contains(connection.ToId)) continue;
                var fulfillRequirementsToMoveToTile = true;
                if (connection.Reqs == null)
                {
                    newTilesWithBorder.Add(connection.ToId);
                    continue;
                }

                foreach (var req in connection.Reqs)
                {
                    var requirement = RequirementMovementStore.GetRequirementById(req);
                    if (requirement == null) continue;

                    bool fulfillThisRequirement = requirement.CheckRequirements(player);
                    if (!fulfillThisRequirement)
                    {
                        fulfillRequirementsToMoveToTile = false;
                        break;
                    }
                }
                if (fulfillRequirementsToMoveToTile)
                {
                    newTilesWithBorder.Add(connection.ToId);
                }
            }

            // Assign the new collection to force UI updates
            TilesWithBorder = newTilesWithBorder;
        }

        public void SetRotateConnections()
        {
            var tiles = _game.GameVisualManager.GameTiles.Tiles;
            var CurrentTile = _game.PawnManager.CurrentTile;
            var newTilesWithBorder = new ObservableCollection<int>();
            foreach (var tile in tiles)
            {
                if (tile.RotateID == CurrentTile.RotateID && tile.Id != CurrentTile.Id)
                {
                    newTilesWithBorder.Add(tile.Id);
                }
            }
            TilesWithBorder = newTilesWithBorder;
        }

        public void SetTeleportConnections()
        {
            var tiles = _game.GameVisualManager.GameTiles.Tiles;
            var CurrentTile = _game.PawnManager.CurrentTile;
            var newTilesWithBorder = new ObservableCollection<int>();
            foreach (var tile in tiles)
            {
                if (tile.TileType.Id == TileHelper.MagicTileId && tile.Id != CurrentTile.Id)
                {
                    newTilesWithBorder.Add(tile.Id);
                }
            }
            TilesWithBorder = newTilesWithBorder;
        }

        public void SetBlockTilesConnections()
        {
            var tiles = _game.GameVisualManager.GameTiles.Tiles;
            var CurrentTile = _game.PawnManager.CurrentTile;
            var newTilesWithBorder = new ObservableCollection<int>();
            foreach (var tile in tiles)
            {
                if (tile.TileType.Id != TileHelper.MagicTileId&& 
                    tile.Id != CurrentTile.Id &&
                    tile.TileType.Id != TileHelper.CastleTileId &&
                    tile.TileType.Id != TileHelper.StartTileId &&
                    tile.Token == null)
                {
                    newTilesWithBorder.Add(tile.Id);
                }
            }
            TilesWithBorder = newTilesWithBorder;
        }

        public void RemoveAvailableConnections()
        {
            var newTilesWithBorder = new ObservableCollection<int>();
            TilesWithBorder = newTilesWithBorder;
        }

        public bool CheckIfTileAccessible(int tileId)
        {
            return TilesWithBorder.Contains(tileId);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

