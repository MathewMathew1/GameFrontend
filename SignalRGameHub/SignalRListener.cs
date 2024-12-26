using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Models;
using BoardGameFrontend.Managers;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading.Tasks;
using System;
using System.Windows;
using BoardGameFrontend.ChatLogManager;
using BoardGameFrontend.Services;

namespace BoardGameFrontend.SignalHub
{
    public class SignalRListener : IDisposable
    {
        private readonly HubConnection _connection;
        private readonly Game _game;
        private readonly LobbyViewModelManager _lobbyVM;
        private readonly Dispatcher _dispatcher;
        private readonly TabManager _tabManager;

        private HeroCardsListener _heroCardsListener;
        private LobbyListener _lobbyListener;
        private ArtifactsListener _artifactsListener;
        private MercenariesListener _mercenariesListener;
        private PhasesListener _phasesListener;
        private MiniPhasesListener _miniPhasesListener;
        private OtherEventsListener _otherEventsListener;
        private TileListener _tileListener;
        private RoyalCardListener _royalCardListener;
        
        private bool _disposed;

        public SignalRListener(HubConnection connection, Game game, LobbyViewModelManager lobbyVM, Dispatcher dispatcher, TabManager tabManager)
        {
            _connection = connection;
            _game = game;
            _lobbyVM = lobbyVM;
            _dispatcher = dispatcher;
            _tabManager = tabManager;
            _disposed = false;
        }

        public void InitializeListeners(MainChatLogManager mainChatLogManager, ServiceLobbyWindow serviceLobbyWindow, LobbyWindow lobbyWindow)
        {
            _heroCardsListener = new HeroCardsListener(_connection, _game, mainChatLogManager, _dispatcher);
            _lobbyListener = new LobbyListener(_connection, _game, _lobbyVM, mainChatLogManager, _dispatcher, serviceLobbyWindow, lobbyWindow);
            _artifactsListener = new ArtifactsListener(_connection, _game, mainChatLogManager, _dispatcher);
            _mercenariesListener = new MercenariesListener(_connection, _game, mainChatLogManager, _dispatcher);
            _phasesListener = new PhasesListener(_connection, _game, mainChatLogManager, _dispatcher, _tabManager);
            _miniPhasesListener = new MiniPhasesListener(_connection, _game, mainChatLogManager, _dispatcher, _tabManager);
            _otherEventsListener = new OtherEventsListener(_connection, _game, mainChatLogManager, _dispatcher, _lobbyVM);
            _tileListener = new TileListener(_connection, _game, mainChatLogManager, _dispatcher);
            _royalCardListener = new RoyalCardListener(_connection, _game, mainChatLogManager, _dispatcher);
        }

        public async Task StartConnection()
        {
            try
            {
                await _connection.StartAsync();
                await _connection.InvokeAsync("JoinLobby", _lobbyVM.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to SignalR hub: {ex.Message}");
                await Task.Delay(5000);
                await StartConnection();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                try
                {
                    // Dispose the HubConnection
                    

                    // Dispose each listener
                    _heroCardsListener?.Dispose();
                    _lobbyListener?.Dispose();
                    _artifactsListener?.Dispose();
                    _mercenariesListener?.Dispose();
                    _phasesListener?.Dispose();
                    _miniPhasesListener?.Dispose();
                    _otherEventsListener?.Dispose();
                    _tileListener?.Dispose();
                    _royalCardListener?.Dispose();

                    await _connection.StopAsync();
                    await _connection.DisposeAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error disposing SignalR connection: {ex.Message}");
                }
            }

            _disposed = true;
        }

        public async Task Disconnect()
        {
            try
            {
                await _connection.InvokeAsync("Disconnect");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to SignalR hub: {ex.Message}");
                await Task.Delay(5000);
            }
        }
    }
}
