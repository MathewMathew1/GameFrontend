using System.Windows;
using System.Windows.Controls;
using BoardGameFrontend.Models;
using System;
using BoardGameFrontend.Helpers;

namespace BoardGameFrontend.Behaviors
{
    public static class DropBehavior
    {
        // Attached property to enable drop behavior
        public static readonly DependencyProperty IsDropEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsDropEnabled",
                typeof(bool),
                typeof(DropBehavior),
                new PropertyMetadata(false, OnIsDropEnabledChanged));

        public static bool GetIsDropEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDropEnabledProperty);
        }

        public static void SetIsDropEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDropEnabledProperty, value);
        }

        private static void OnIsDropEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uiElement)
            {
                if ((bool)e.NewValue)
                {

                    uiElement.AllowDrop = true;
                    uiElement.Drop += OnDrop;
                }
                else
                {

                    uiElement.AllowDrop = false;
                    uiElement.Drop -= OnDrop;
                }
            }
        }


        private static async void OnDrop(object sender, DragEventArgs e)
        {
            if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;
            var game = window.Game;
            game.GameVisualManager.TilesBorderManager.RemoveAvailableConnections();

            if (!(sender is Border border)) return;

            var droppedData = e.Data.GetData(typeof(Image)) as Image;
            if (droppedData == null) return;

            var droppedTile = border.DataContext as TileWithType;
            if (droppedTile == null) return;

            var tileId = droppedTile.Id;

            if(game.MiniPhaseManager.CurrentPhase?.Name == MiniPhaseType.TeleportationPhase){
                
                if(droppedTile.TileType.Id != TileHelper.MagicTileId || droppedTile.Id == game.PawnManager.CurrentTile.Id) return;
                
                var lobbyService = window.LobbyService;
                await lobbyService.TeleportToTile(tileId);
                return;
            }

            var movedToTile = game.PawnManager.SetCurrentTile(game.GameVisualManager.GameTiles.GetTileById(tileId));
            if (!movedToTile) return;

            if(game.PawnManager.TeleportationIsOn){
                game.PawnManager.TeleportationIsOn = false;
                var lobbyService = window.LobbyService;

                await lobbyService.MoveToTile(game.PawnManager.CurrentTile.Id, true, false, tileId);
            }

           
        }
    }
}
