using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Models;
using BoardGameFrontend.Managers;
using BoardGameFrontend.ChatLogManager;
using System;
using System.Windows.Threading;

namespace BoardGameFrontend.SignalHub
{
    public class RoyalCardListener : IDisposable
    {
        private readonly HubConnection _connection;
        private bool _disposed;

        public RoyalCardListener(HubConnection connection, Game game, MainChatLogManager mainChatLogManager, Dispatcher dispatcher)
        {
            _connection = connection;
            _disposed = false;

            // Register listeners
            _connection.On<RoyalCardPlayed>("RoyalCardPlayed", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.RoyalCardPlayed(data);
                    mainChatLogManager.RoyalCardChatLogManager.RoyalCardPlayed(data);
                });
            });

            _connection.On<BanishRoyalCardEventData>("BanishRoyalCardEvent", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.BanishRoyalCar(data);
                    mainChatLogManager.RoyalCardChatLogManager.RoyalCardBanished(data);
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
                _connection.Remove("RoyalCardPlayed");
                _connection.Remove("BanishRoyalCardEvent");
            }

            _disposed = true;
        }
    }
}
