using System.ComponentModel;
using BoardGameFrontend.Models;
using System.Windows.Controls;
using System.Windows;
using BoardGameFrontend.Helpers;
using System;



namespace BoardGameFrontend.Managers
{
    public class PawnManager : INotifyPropertyChanged
    {
        private Game _game;
        public bool TeleportationIsOn;
        private Image? _pawnImage;
        private int _stackPanelLeft;
        private int _stackPanelTop;
        private TileWithType _currentTile;
        public TileWithType CurrentTile
        {
            get => _currentTile;
            set
            {
                if (_currentTile != value)
                {
                    _currentTile = value;
                    OnPropertyChanged(nameof(CurrentTile));
                    UpdatePawnPosition();
                    SetAvailableAdjacentMovement();
                    UpdateStackPosition();
                }
            }
        }

        public int StackPanelLeft
        {
            get => _stackPanelLeft;
            set
            {
                if (_stackPanelLeft != value)
                {
                    _stackPanelLeft = value;
                    OnPropertyChanged(nameof(StackPanelLeft));
                }
            }
        }

        public int StackPanelTop
        {
            get => _stackPanelTop;
            set
            {
                if (_stackPanelTop != value)
                {
                    _stackPanelTop = value;
                    OnPropertyChanged(nameof(StackPanelTop));
                }
            }
        }

        private void UpdateStackPosition()
        {
            const int canvasWidth = 2000;
            const int canvasHeight = 2000;
            const int stackPanelWidth = 450;
            const int stackPanelHeight = 70;

            if (_currentTile != null)
            {
                StackPanelLeft = Math.Min(canvasWidth - stackPanelWidth, Math.Max(0, _currentTile.OffsetX));
                StackPanelTop = Math.Min(canvasHeight - stackPanelHeight, Math.Max(0, _currentTile.OffsetY));
            }
        }

        public void SetPawnImage(Image pawnImage)
        {
            _pawnImage = pawnImage;
        }

        private TileWithType _previousTile;
        public TileWithType PreviousTile
        {
            get => _previousTile;
            set
            {
                if (_previousTile != value)
                {
                    _previousTile = value;
                    OnPropertyChanged(nameof(PreviousTile));
                }
            }
        }

        private bool _showBoots;
        public bool ShowBoots
        {
            get => _showBoots;
            set
            {
                if (_showBoots != value)
                {
                    _showBoots = value;
                    OnPropertyChanged(nameof(ShowBoots));
                }
            }
        }

        private bool _fullBootAvailable;
        public bool FullBootAvailable
        {
            get => _fullBootAvailable;
            set
            {
                if (_fullBootAvailable != value)
                {
                    _fullBootAvailable = value;
                    SetAvailableAdjacentMovement();
                    OnPropertyChanged(nameof(FullBootAvailable));
                }
            }
        }

        private bool _adjacentMovementAvailable;
        public bool AdjacentMovementAvailable
        {
            get => _adjacentMovementAvailable;
            set
            {
                if (_adjacentMovementAvailable != value)
                {
                    _adjacentMovementAvailable = value;
                    OnPropertyChanged(nameof(AdjacentMovementAvailable));
                }
            }
        }

        private void SetAvailableAdjacentMovement(){
            AdjacentMovementAvailable = FullBootAvailable && CurrentTile.TileType.Id != TileHelper.MagicTileId;
        }

        private bool _fullUnBootAvailable;
        public bool UnFullBootAvailable
        {
            get => _fullUnBootAvailable;
            set
            {
                if (_fullUnBootAvailable != value)
                {
                    _fullUnBootAvailable = value;
                    OnPropertyChanged(nameof(UnFullBootAvailable));
                }
            }
        }

        public void UpdatePawnPosition()
        {
            if (_pawnImage == null) return;

            double newLeft = _currentTile.OffsetX / 4;
            double newTop = _currentTile.OffsetY / 4;

            Point adjustedPosition = ImagePositionHelper.AdjustPosition(newLeft, newTop);

            Canvas.SetLeft(_pawnImage, adjustedPosition.X);
            Canvas.SetTop(_pawnImage, adjustedPosition.Y);
        }

        public PawnManager(TileWithType tile, Game game)
        {
            _currentTile = tile;
            _previousTile = tile;
            _showBoots = false;
            _game = game;
        }

        public void ShowVisualInfo()
        {
            _game.GameVisualManager.TilesBorderManager.SetAvailableConnections();
        }

        public bool SetCurrentTile(TileWithType tile)
        {
            if (TeleportationIsOn)
            {
                var canTeleportHere = tile.TileType.Id == TileHelper.MagicTileId && tile.Id != CurrentTile.Id 
                    && !tile.IsInRangeOfCastle(_game.UserControlledPlayer.PlayerHeroCardManager.CurrentHeroCard!.HeroCard.Faction, 3);

                return canTeleportHere;
            }

            if (!CanMoveToTile(tile)) return false;

            var fullBoot = _game.UserControlledPlayer.PlayerHeroCardManager.CurrentHeroCard?.MovementFullLeft > 0 || false;
            var unFullBoot = _game.UserControlledPlayer.PlayerHeroCardManager.CurrentHeroCard?.MovementUnFullLeft > 0 || false;

            if (!(fullBoot || unFullBoot)) return false;

            FullBootAvailable = fullBoot;

            if(tile.TileType.Req != null){
                var req = RequirementMovementStore.GetRequirementById(tile.TileType.Req.Value);

                if(req == null){
                    FullBootAvailable = false;
                }
                else{
                    FullBootAvailable = req.CheckRequirements(_game.UserControlledPlayer);
                }
                
            }
 
            UnFullBootAvailable = true;

            PreviousTile = CurrentTile;
            CurrentTile = tile;
            ShowBoots = true;

            return true;
        }

        public void MoveToTile(TileWithType tile)
        {
            CurrentTile = tile;
            PreviousTile = tile;
            ShowBoots = false;
        }


        public void CancelMovement()
        {
            CurrentTile = PreviousTile;
            ShowBoots = false;
            TeleportationIsOn = false;
        }

        public void SetupTeleportation()
        {
            TeleportationIsOn = true;
            ShowBoots = false;
        }

        public void CheckForTeleport(int? TeleportationTileId)
        {
            if (TeleportationTileId != null)
            {
                CurrentTile = _game.GameVisualManager.GameTiles.GetTileById(TeleportationTileId.Value);
            }
        }


        private bool CanMoveToTile(TileWithType tile)
        {
            if (!PlayerCanMove()) return false;

            if (_game.UserControlledPlayer.PlayerHeroCardManager.CurrentHeroCard!.VisitedPlaces.Contains(tile.Id)) return false;

            bool fulfillRequirementsToMoveToTile = true;
            bool thereIsConnection = false;

            foreach (var connection in CurrentTile.Connections)
            {
                if (connection.ToId == tile.Id)
                {
                    thereIsConnection = true;
                    if (connection.Reqs == null) continue;
                    foreach (var req in connection.Reqs)
                    {
                        var requirement = RequirementMovementStore.GetRequirementById(req);
                        if(requirement == null) continue;

                        bool fulfillThisRequirement = requirement.CheckRequirements( _game.UserControlledPlayer);
                        if (!fulfillThisRequirement)
                        {
                            fulfillRequirementsToMoveToTile = false;
                            break;
                        }
                    }
                    if (!fulfillRequirementsToMoveToTile) break;
                }
            }

            return fulfillRequirementsToMoveToTile && thereIsConnection;
        }

        public bool PlayerCanMove()
        {
            if(_game.IsUserControlledPlayersTurn == true && _game.MiniPhaseManager.CurrentPhase?.Name == MiniPhaseType.TeleportationPhase) return true;

            bool gameHasStarted = _game.GameHasStarted;
            if (!gameHasStarted) return false;

            bool isCurrentPlayerTurn = _game.IsUserControlledPlayersTurn;
            if (!isCurrentPlayerTurn) return false;

            bool isBoardPhase = _game.PhaseManager.CurrentPhase.Name == PhaseType.BoardPhase;
            if (!isBoardPhase) return false;

            if(_game.MiniPhaseManager.CurrentPhase != null) return false;

            if (ShowBoots) return false;

            if(_game.UserControlledPlayer.PlayerHeroCardManager.CurrentHeroCard == null) return false;

            
            var noMovementLeft = _game.UserControlledPlayer.PlayerHeroCardManager.CurrentHeroCard.MovementUnFullLeft == 0 
                && _game.UserControlledPlayer.PlayerHeroCardManager.CurrentHeroCard.MovementFullLeft == 0;

            if (noMovementLeft) return false;

            return true;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }


}