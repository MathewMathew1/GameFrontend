using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Models;
using BoardGameFrontend.Managers;
using BoardGameFrontend.ChatLogManager;
using System.Windows.Threading;
using System;


namespace BoardGameFrontend.SignalHub
{
    public class TileListener: IDisposable
    {
        private readonly HubConnection _connection;
        private bool _disposed;


        public TileListener(HubConnection connection, Game game, MainChatLogManager mainChatLogManager, Dispatcher dispatcher)
        {
            _connection = connection;
            _disposed = false;
            connection.On<MoveOnTileData>("MoveToTile", (moveOnTileData) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.MoveOnTile(moveOnTileData);
                    mainChatLogManager.TileChatLogManager.MoveOnTile(moveOnTileData);
                });
            });

            connection.On<TileRewardData>("TileReward", (tileRewardData) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.TileReward(tileRewardData);
                    mainChatLogManager.TileChatLogManager.TileReward(tileRewardData);
                });
            });

            connection.On<MoveOnTileOnEvent>("MoveOnTileOnEvent", (moveOnTileData) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.MoveByAura(moveOnTileData);
                    mainChatLogManager.TileChatLogManager.MoveOnTileOnEvent(moveOnTileData);
                });
            });

            connection.On<BlockedTileData>("BlockedTileEvent", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.BlockTile(data);
                    mainChatLogManager.TileChatLogManager.BlockedTileEvent(data);
                });
            });

            connection.On<GoldIntoMovementEventData>("GoldIntoMovementEvent", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.GoldIntoMovementEvent(data);
                    mainChatLogManager.TileChatLogManager.GoldIntoMovementEvent(data);
                });
            });

            connection.On<FullMovementIntoEmptyEventData>("FullMovementIntoEmptyEvent", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.MovementsConvertedEvent(data);
                    mainChatLogManager.TileChatLogManager.MovementConvertedEvent(data);
                });
            });

            connection.On<ResourceReceivedEventData>("ResourceReceivedEvent", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.AddResource(data);
                    mainChatLogManager.TileChatLogManager.ReceivedResource(data);
                });
            });

            connection.On<SwapTokensDataEventData>("SwapTokensDataEvent", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.SwapTokens(data);
                    mainChatLogManager.TileChatLogManager.SwappedTokens(data);
                });
            });

            connection.On<RotateTileEventData>("RotateTileDataEvent", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.RotatePawn(data);
                    mainChatLogManager.TileChatLogManager.RotatePawn(data);
                });
            });


        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _connection.Remove("RotateTileDataEvent");
                _connection.Remove("SwapTokensDataEvent");
                _connection.Remove("ResourceReceivedEvent");
                _connection.Remove("FullMovementIntoEmptyEvent");
                _connection.Remove("BlockedTileEvent");
                _connection.Remove("MoveToTile");
                _connection.Remove("TileReward");
                _connection.Remove("MoveOnTileOnEvent");        
            }

            _disposed = true;
        }
    }
}