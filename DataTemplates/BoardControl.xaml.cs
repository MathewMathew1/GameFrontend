using BoardGameFrontend.Commands;
using BoardGameFrontend.Helpers;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BoardGameFrontend.Windows
{
    public partial class BoardControl : UserControl
    {
        bool _preventRequest = false;

        public BoardControl()
        {
            InitializeComponent();
            SetupCanvasForPawn();
        }

        public ICommand YourCommand => new RelayCommandWithTypes<int>(ExecuteCommand);
        public ICommand SwapTokensCommand => new RelayCommand(async _ => await SwapTokens());

        private void SelectToken(int parameter)
        {
            if (_preventRequest) return;
            if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;

            var game = window.Game;
            if (game.MiniPhaseManager.CurrentPhase?.Name != MiniPhaseType.SwapTokens || !game.IsUserControlledPlayersTurn) return;

            game.TokenManager.SelectTokenByTileId(parameter);
        }

        private void ExecuteCommand(int parameter)
        {
            SelectToken(parameter);
            BlockTile(parameter);   
            RotatePawn(parameter);  
        }

        private async Task SwapTokens()
        {
            if (_preventRequest) return;
            if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;

            var game = window.Game;

            if (game.MiniPhaseManager.CurrentPhase?.Name != MiniPhaseType.SwapTokens || !game.IsUserControlledPlayersTurn) return;

            _preventRequest = true;

            var lobbyService = window.LobbyService;

            var tileIdOne = game.TokenManager.SelectedTokensId[0];
            var tileIdTwo = game.TokenManager.SelectedTokensId[1];

            await lobbyService.SwapToken(tileIdOne, tileIdTwo);

            _preventRequest = false;
        }

        private async Task RotatePawn(int parameter)
        {
            if (_preventRequest) return;
            if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;

            var game = window.Game;

            if (game.MiniPhaseManager.CurrentPhase?.Name != MiniPhaseType.RotatePawnMiniPhase || !game.IsUserControlledPlayersTurn) return;

            _preventRequest = true;

            var lobbyService = window.LobbyService;

            await lobbyService.RotatePawn(parameter);

            _preventRequest = false;
        }

        

        private async void BlockTile(int parameter)
        {
            if (_preventRequest) return;
            if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;

            var game = window.Game;
            if (game.MiniPhaseManager.CurrentPhase?.Name != MiniPhaseType.BlockTilePhase || !game.IsUserControlledPlayersTurn) return;

            if(!game.GameVisualManager.TilesBorderManager.TilesWithBorder.Contains(parameter)){
                return;
            }

            _preventRequest = true;

            var lobbyService = window.LobbyService;

            await lobbyService.BlockTile(parameter);

            _preventRequest = false;
        }

        public void SetupCanvasForPawn()
        {
            if (Application.Current.MainWindow is LobbyWindow window && window.Game != null)
            {
                var game = window.Game;
                game.PawnManager.SetPawnImage(draggableImage);
            }
        }

        public void CancelMovement_Click(object sender, RoutedEventArgs e)
        {
            if (_preventRequest) return;
            if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;

            var game = window.Game;
            game.PawnManager.CancelMovement();
        }

        public async void UnFullMovement_Click(object sender, RoutedEventArgs e)
        {
            if (_preventRequest) return;
            if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;

            _preventRequest = true;
            var game = window.Game;
            var lobbyService = window.LobbyService;

            var successfulMovement = await lobbyService.MoveToTile(game.PawnManager.CurrentTile.Id, false, false);
            _preventRequest = false;
            game.PawnManager.ShowBoots = false;
            if (successfulMovement == false)
            {
                game.PawnManager.CancelMovement();
                return;
            }


        }


        public async void FullMovement_Click(object sender, RoutedEventArgs e)
        {
            if (_preventRequest) return;
            if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;
            var game = window.Game;

            if (game.PawnManager.CurrentTile.TileType.Id == TileHelper.MagicTileId)
            {
                game.PawnManager.SetupTeleportation();
                return;
            }

            _preventRequest = true;

            var lobbyService = window.LobbyService;

            var successfulMovement = await lobbyService.MoveToTile(game.PawnManager.CurrentTile.Id, true, false);
            _preventRequest = false;
            game.PawnManager.ShowBoots = false;
            if (successfulMovement == false)
            {
                game.PawnManager.CancelMovement();
                return;
            }
        }

        public async void AdjacentMovement_Click(object sender, RoutedEventArgs e)
        {
            if (_preventRequest) return;
            if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;
            var game = window.Game;

            if (game.PawnManager.CurrentTile.TileType.Id == TileHelper.MagicTileId)
            {
                game.PawnManager.SetupTeleportation();
                return;
            }

            _preventRequest = true;

            var lobbyService = window.LobbyService;

            var successfulMovement = await lobbyService.MoveToTile(game.PawnManager.CurrentTile.Id, true, true);
            _preventRequest = false;
            game.PawnManager.ShowBoots = false;
            if (successfulMovement == false)
            {
                game.PawnManager.CancelMovement();
                return;
            }
        }
    }
}