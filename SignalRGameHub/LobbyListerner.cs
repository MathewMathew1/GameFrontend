using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Models;
using System;
using System.Windows.Threading;
using System.Windows;
using BoardGameFrontend.ChatLogManager;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Services;
using BoardGameFrontend.AutoMapper;

namespace BoardGameFrontend.SignalHub
{
    public class LobbyListener : IDisposable
    {
        private readonly HubConnection _connection;
        private readonly Game _game;
        private readonly LobbyViewModelManager _lobbyVM;
        private readonly MainChatLogManager _mainChatLogManager;
        private readonly Dispatcher _dispatcher;
        private readonly ServiceLobbyWindow _serviceLobbyWindow;
        private readonly LobbyWindow _lobbyWindow;
        private bool _disposed;

        public LobbyListener(HubConnection connection, Game game, LobbyViewModelManager lobbyVM, MainChatLogManager mainChatLogManager, Dispatcher dispatcher, ServiceLobbyWindow serviceLobbyWindow, LobbyWindow lobbyWindow)
        {
            _connection = connection;
            _game = game;
            _lobbyVM = lobbyVM;
            _mainChatLogManager = mainChatLogManager;
            _dispatcher = dispatcher;
            _serviceLobbyWindow = serviceLobbyWindow;
            _lobbyWindow = lobbyWindow;
            _disposed = false;

            // Register SignalR listeners
            RegisterListeners();
        }

        private void RegisterListeners()
        {
            _connection.On<PlayerViewModel>("PlayerJoined", (player) =>
            {
                _dispatcher.Invoke(() =>
                {
                    _lobbyVM.AddPlayer(new PlayerViewModel { Name = player.Name, Id = player.Id });
                    _mainChatLogManager.LobbyChatLogManager.PlayerJoined(player);
                });
            });

            _connection.On<PlayerViewModel>("PlayerLeft", (player) =>
            {
                _dispatcher.Invoke(() =>
                {
                    _lobbyVM.RemovePlayerById(player.Id);
                    _mainChatLogManager.LobbyChatLogManager.PlayerLeft(player);
                });
            });

            _connection.On<PlayerViewModel>("DestroyLobby", (player) =>
            {
                _dispatcher.Invoke(() =>
                {
                    _mainChatLogManager.LobbyChatLogManager.DestroyedLobby(player);
                    MessageBox.Show("Lobby has been destroyed.");
                    _connection.StopAsync();

                    var mainWindow = new MainViewWindow();
                    mainWindow.Show();
                    _lobbyWindow.Close();
                });
            });

            _connection.On<User, string>("ReceiveMessage", (user, message) =>
            {
                _dispatcher.Invoke(() =>
                {
                    var player = AutoMapperConfig.Mapper.Map<PlayerViewModel>(user);
                    _mainChatLogManager.LobbyChatLogManager.MessageSend(player, message);
                });
            });

            _connection.On<StartGameModel>("UpdateLobbyInfo", (data) =>
            {
                _dispatcher.Invoke(() =>
                {
                    _serviceLobbyWindow.UpdateStartGameData(data);
                });
            });

            _connection.On<PlayerViewModel>("PlayerDisconnected", (data) =>
            {
                _dispatcher.Invoke(() =>
                {
                    if (!_game.GameHasStarted)
                    {
                        _lobbyVM.RemovePlayerById(data.Id);
                    }
                    _mainChatLogManager.LobbyChatLogManager.PlayerDisconnected(data);
                    _game.ConnectPlayer(false, data);
                });
            });

            _connection.On<PlayerViewModel>("PlayerRejoined", (data) =>
            {
                _dispatcher.Invoke(() =>
                {
                    _mainChatLogManager.LobbyChatLogManager.PlayerConnected(data);
                    _game.ConnectPlayer(true, data);
                });
            });

            _connection.On<FullGameData>("PlayerRejoinedData", (data) =>
            {
                _dispatcher.Invoke(() =>
                {
                    _game.SetData(data);
                    _lobbyVM.GameHaveStarted = true;
                });
            });

            _connection.On<StartOfGame>("GameStarted", (gameStarted) =>
            {
                _dispatcher.Invoke(() =>
                {
                    try
                    {
                        _game.StartGame(gameStarted);
                        _lobbyVM.GameHaveStarted = true;
                        _mainChatLogManager.LobbyChatLogManager.GameStarted();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
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
                _connection.Remove("PlayerJoined");
                _connection.Remove("PlayerLeft");
                _connection.Remove("DestroyLobby");
                _connection.Remove("ReceiveMessage");
                _connection.Remove("UpdateLobbyInfo");
                _connection.Remove("PlayerDisconnected");
                _connection.Remove("PlayerRejoined");
                _connection.Remove("PlayerRejoinedData");
                _connection.Remove("GameStarted");
            }

            _disposed = true;
        }
    }
}
