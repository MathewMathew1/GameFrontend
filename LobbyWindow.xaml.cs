using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Helpers;
using BoardGameFrontend.Models;
using BoardGameFrontend.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Services;
using System.Windows.Controls;
using BoardGameFrontend.SignalHub;
using BoardGameFrontend.ChatLogManager;
using System.Windows.Input;
using System.Text.Json.Serialization;

namespace BoardGameFrontend
{
    public partial class LobbyWindow : FullScreenWindow
    {
        private HubConnection _connection;
        private HttpClient _httpClient = new HttpClient();
        private MusicManager _musicManager = new MusicManager();
        public LobbyViewModelManager LobbyVM { get; set; }
        public UserDataManager UserDataManager { get; set; }
        public ServiceLobbyWindow LobbyService { get; }
        public Game Game { get; set; } = new Game();
        public MainChatLogManager MainChatLogManager { get; set; }
        public SignalRListener SignalRListener { get; set; }
        public ChatGameManager ChatGameManager { get; set; }
        public ChatLobbyManager ChatLobbyManager { get; set; }
        public TabManager TabManager { get; set; }
        private GameOptions _gameOptions;
        private bool _intentionalDisconnect = false;

        private StringStorage _stringStorage = new StringStorage();
        private readonly string CHAT_LOBBY = "Chat";


        public LobbyWindow(string lobbyId)
        {
            LobbyService = new ServiceLobbyWindow(_httpClient, lobbyId);
            LobbyVM = new LobbyViewModelManager(new LobbyDetailedViewModel
            {
                LobbyName = "loading data",
                Id = "Loading Data",
                HostId = new Guid(),
                Players = new List<PlayerViewModel> { }
            });
            ChatGameManager = new ChatGameManager(LobbyVM);
            ChatLobbyManager = new ChatLobbyManager(LobbyVM);

            DataContext = this;

            var data = UserHelpers.LoadUserData()!;
            _connection = new HubConnectionBuilder()
                    .WithUrl($"{UrlHelper.GetBaseUrlSignalR()}", options =>
                    {
                        options.AccessTokenProvider = async () => data.Token;
                    })
                    .WithAutomaticReconnect(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10)
                    })
                    .Build();

            UserDataManager = new UserDataManager(data);
            if (data != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", data.Token);
                LobbyVM.CurrentUserId = data.Id;
                Game.PlayerId = data.Id;
            }
            else
            {
                Close();
            }

            LoadLobbyData(lobbyId);

            _gameOptions = GameOptions.LoadFromFile();
            TabManager = new TabManager(Game, _gameOptions.AutomaticSwitchBetweenTabs);
            this.PreviewKeyDown += new KeyEventHandler(OnKeyDown);

            SignalRListener = new SignalRListener(_connection, Game, LobbyVM, Dispatcher, TabManager);
            InitializeSignalR();

            MainChatLogManager = new MainChatLogManager(ChatLobbyManager, ChatGameManager, Game);
            SignalRListener!.InitializeListeners(MainChatLogManager, LobbyService, this);

            this.Closing += LobbyWindow_Closing;

            InitializeComponent();

            TabManager.SetupTab(myTabControl);

        }

        private async void LobbyWindow_Closing(object sender, CancelEventArgs e)
        {
            _intentionalDisconnect = true;


            _musicManager.ClearMediaPlayer();
            SignalRListener?.Dispose();

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void InitializeSignalR()
        {
            
            _connection.KeepAliveInterval = TimeSpan.FromSeconds(5); // Ping server every 5 seconds

            _connection.Closed += async (error) =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    if (!_intentionalDisconnect)  // Only show if not intentional
                    {
                        MessageBox.Show("Connection could not be restored. Please check your network and try again.", "Disconnected", MessageBoxButton.OK, MessageBoxImage.Error);
                        var mainWindow = new MainViewWindow();
                        mainWindow.Show();
                        Close();
                    }
                });
            };

            _connection.Reconnecting += async (error) =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    if (!_intentionalDisconnect)  // Only show if not intentional
                    {
                        MessageBox.Show("Connection interrupted. Attempting to reconnect...", "Reconnecting", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                });
            };

            _connection.Reconnected += async (connectionId) =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    _connection.InvokeAsync("Reconnect", LobbyVM.Id);

                    MessageBox.Show("Reconnected successfully!", "Connection Restored", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            };

            await SignalRListener.StartConnection();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {
                var selectedTab = (TabItem)e.AddedItems[0]!;
                if (selectedTab.Header.ToString() == "Hero Cards")
                {

                }
                else if (selectedTab.Header.ToString() == "Board")
                {

                }
            }

        }

        private async void LoadLobbyData(string lobbyId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{UrlHelper.GetBaseUrl()}lobby/{lobbyId}");
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                    };
                    var responseData = await response.Content.ReadAsStringAsync()!;
                    var data = JsonSerializer.Deserialize<LobbyManagerInfo>(responseData, options)!;
                    LobbyService.UpdateStartGameData(data.StartGameModel);
                    LobbyVM.UpdateLobby(data.Lobby);
                }
                else
                {
                    MessageBox.Show("Failed to load lobby data.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }




        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var message = ChatInputTextBox.Text;
                if (message.Length == 0)
                {

                    return;
                }

                if (_stringStorage.ContainsString(CHAT_LOBBY)) return;
                ChatInputTextBox.Clear();

                var content = new StringContent(JsonSerializer.Serialize(new { Message = message }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}lobby/message/{LobbyVM.Id}", content);
                _stringStorage.RemoveString(CHAT_LOBBY);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to send message.");
                }
            }
            catch (Exception ex)
            {
                if (_stringStorage.RemoveString(CHAT_LOBBY))
                    MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private async void PickAHeroCard(object sender, RoutedEventArgs e)
        {
            await _stringStorage.RunIfNotExistsAsync("PickHeroCard", async () =>
            {
                if (Game.HeroCardsBoardManager.SelectedHeroCard == null)
                {
                    MessageBox.Show("Failed to send message.");
                    return;
                }
                var content = new StringContent(JsonSerializer.Serialize(new { HeroCardId = Game.HeroCardsBoardManager.SelectedHeroCard.Id }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}heroCard/take/{LobbyVM.Id}", content);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to send message.");
                }
            });
        }

        private async void LeaveLobby_Click(object sender, RoutedEventArgs e)
        {
            var response = await _httpClient.DeleteAsync($"{UrlHelper.GetBaseUrl()}lobby/leave/{LobbyVM.Id}");
            if (response.IsSuccessStatusCode)
            {
                await _connection.StopAsync();

                var mainWindow = new MainViewWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Failed to leave lobby.");
            }
        }

        private async void DisconnectFromLobby_Click(object sender, RoutedEventArgs e)
        {
            await SignalRListener.Disconnect();

            var mainWindow = new MainViewWindow();
            mainWindow.Show();

            Close();
        }


        private Task Connection_Reconnected(string connectionId)
        {
            MessageBox.Show("Reconnected to the SignalR hub.");
            return Task.CompletedTask;
        }

        private Task Connection_Reconnecting()
        {
            MessageBox.Show("Attempting to reconnect to the SignalR hub...");
            return Task.CompletedTask;
        }

        private Task Connection_Closed(Exception? exception)
        {
            MessageBox.Show("Connection to the SignalR hub has been closed.");
            return Task.CompletedTask;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Escape)
            {
                OpenGameOptionsWindow();
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenGameOptionsWindow();
        }

        private void OpenGameOptionsWindow()
        {
            MainMenuWindow optionsWindow = new MainMenuWindow();

            // Subscribe to the Closed event of the options window
            optionsWindow.Closed += OptionsWindow_Closed;

            optionsWindow.ShowDialog();
        }

        private void OptionsWindow_Closed(object? sender, EventArgs e)
        {

            var gameOptions = GameOptions.LoadFromFile();

            TabManager.ChangeSwitchTabs(gameOptions.AutomaticSwitchBetweenTabs);

        }

        private void ChatInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage_Click(sender, e);

                e.Handled = true;
            }
        }


    }
}