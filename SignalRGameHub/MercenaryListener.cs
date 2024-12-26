using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Models;
using BoardGameFrontend.Managers;
using BoardGameFrontend.ChatLogManager;
using System.Windows.Threading;
using System;

namespace BoardGameFrontend.SignalHub
{
    public class MercenariesListener : IDisposable
    {
        private bool _disposed;
        private readonly HubConnection _connection;

        public MercenariesListener(HubConnection connection, Game game, MainChatLogManager mainChatLogManager, Dispatcher dispatcher)
        {
            _connection = connection;
            _disposed = false;

            connection.On<MercenaryPickedData>("MercenaryPicked", (mercenaryData) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.MercenaryPicked(mercenaryData);
                    mainChatLogManager.MercenaryChatLogManager.MercenaryPicked(mercenaryData);
                });
            });

            connection.On<MercenaryRerolledData>("MercenaryRerolled", (mercenaryData) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.MercenaryManager.MercenaryRerolled(mercenaryData);
                    mainChatLogManager.MercenaryChatLogManager.MercenaryRerolled(mercenaryData);
                });
            });



            connection.On<BuyableMercenariesRefreshed>("BuyableMercenariesRefreshed", (buyableMercenariesRefreshed) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.RefreshBuyableMercenaries(buyableMercenariesRefreshed);
                    mainChatLogManager.MercenaryChatLogManager.BuyableMercenariesRefreshed();
                });
            });

            connection.On<FulfillProphecy>("FulfillProphecy", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.FulfillProphecy(data);
                    mainChatLogManager.MercenaryChatLogManager.FulfillProphecy(data);
                });
            });

            connection.On<LockMercenaryData>("LockMercenary", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.LockMercenary(data);
                    mainChatLogManager.MercenaryChatLogManager.LockMercenary(data);
                });
            });

            connection.On<PreMercenaryRerolled>("PreMercenaryRerolled", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.PreMercenaryReroll(data);
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
                // Unregister SignalR event listeners
                _connection.Remove("LockMercenary");
                _connection.Remove("FulfillProphecy");
                _connection.Remove("MercenaryPicked");
                _connection.Remove("MercenaryRerolled");
                _connection.Remove("BuyableMercenariesRefreshed");
                _connection.Remove("PreMercenaryRerolled");
            }

            _disposed = true;
        }


    }
}