using System.Collections.Generic;
using System.Windows;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using BoardGameFrontend.Models;
using BoardGameFrontend.Helpers;
using BoardGameFrontend.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Input;
using System;
using BoardGameFrontend.Managers;

namespace BoardGameFrontend
{
    public partial class MainViewWindow : FullScreenWindow
    {
        private HttpClient _httpClient = new();
        private StringStorage _stringStorage = new StringStorage();
        private readonly string CREATE_LOBBY = "Create";
      

        public MainViewWindow()
        {
            InitializeComponent();

            var data = UserHelpers.LoadUserData();
            if (data != null)
            {
                UsernameBlock.Text = data.Username;
            }

            if (data != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", data.Token);
            }
            FetchLobbies();
            FetchYourLobbies();
        }

        private void RefreshLobbies_Click(object sender, RoutedEventArgs e)
        {
            FetchLobbies();
        }

        private void NavigateToInstructions(object sender, RoutedEventArgs e)
        {
            var instructionsWindow = new InstructionsWindow();
            instructionsWindow.Owner = this; // Set MainWindow as owner
            instructionsWindow.Show();
            this.Hide();
        }


        private async void FetchYourLobbies()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{UrlHelper.GetBaseUrl()}lobby/myLobbies");
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                    };
                    var responseData = await response.Content.ReadAsStringAsync();
                    var lobbies = JsonSerializer.Deserialize<List<LobbyManagerInfo>>(responseData, options)!;

                    var lobbiesWithHostNames = new List<LobbyViewModelWithHostName>();

                    foreach (var lobbyInfo in lobbies)
                    {
                        var lobby = lobbyInfo.Lobby;
                        var hostName = lobbyInfo.Lobby.Players.FirstOrDefault(player => player.Id == lobbyInfo.Lobby.HostId)?.Name ?? "Bugged";
                        lobbiesWithHostNames.Add(new LobbyViewModelWithHostName
                        {
                            Id = lobby.Id,
                            HostId = lobby.HostId,
                            LobbyName = lobby.LobbyName,
                            HostName = hostName,
                            Players = lobby.Players
                        });
                    }

                    YourLobbiesListBox.ItemsSource = lobbiesWithHostNames;
                }
                else
                {
                    MessageBox.Show("Failed to load your lobbies.");
                }
            }
            catch (Exception ex)
            {

            }


        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CreateLobby_Click(sender, e);

                e.Handled = true;
            }
        }

        private async void FetchLobbies()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{UrlHelper.GetBaseUrl()}lobby/all");
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                    };
                    var responseData = await response.Content.ReadAsStringAsync();
                    var lobbies = JsonSerializer.Deserialize<List<LobbyManagerInfo>>(responseData, options)!;

                    var lobbiesWithHostNames = new List<LobbyViewModelWithHostName>();

                    foreach (var lobbyInfo in lobbies)
                    {
                        var lobby = lobbyInfo.Lobby;
                        var hostName = lobbyInfo.Lobby.Players.FirstOrDefault(player => player.Id == lobbyInfo.Lobby.HostId)?.Name ?? "Bugged";
                        lobbiesWithHostNames.Add(new LobbyViewModelWithHostName
                        {
                            Id = lobby.Id,
                            HostId = lobby.HostId,
                            LobbyName = lobby.LobbyName,
                            HostName = hostName,
                            Players = lobby.Players
                        });
                    }

                    LobbiesListBox.ItemsSource = lobbiesWithHostNames;
                }
                else
                {
                    MessageBox.Show("Failed to load lobbies.");
                }
            }
            catch (Exception ex)
            {

            }

        }


        private async void CreateLobby_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_stringStorage.ContainsString(CREATE_LOBBY)) return;

                var playerName = PlayerNameBox.Text;

                if (!string.IsNullOrEmpty(playerName))
                {
                    var content = new StringContent(JsonSerializer.Serialize(new { LobbyName = playerName }), Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}lobby/create", content);
                    _stringStorage.RemoveString(CREATE_LOBBY);
                    if (response.IsSuccessStatusCode)
                    {
                        // Optionally, you might want to handle the response, e.g., get the lobby ID
                        var responseData = await response.Content.ReadAsStringAsync();
                        var lobbyId = JsonSerializer.Deserialize<LobbyResponse>(responseData)!.lobbyId;

                        // Open the LobbyWindow and pass the lobbyId and playerName
                        var lobbyWindow = new LobbyWindow(lobbyId);
                        lobbyWindow.Show();

                        Close(); // or this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create lobby.");
                    }
                }
                else
                {
                    _stringStorage.RemoveString(CREATE_LOBBY);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void LeaveLobby_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button; // Cast sender to Button
            var lobbyId = button?.CommandParameter as string; // Get the CommandParameter
            var response = await _httpClient.DeleteAsync($"{UrlHelper.GetBaseUrl()}lobby/leave/{lobbyId}");
            if (response.IsSuccessStatusCode)
            {
                FetchYourLobbies();
            }
            else
            {
                MessageBox.Show("Failed to leave lobby.");
            }
        }

        private async void JoinLobby_Click(object sender, RoutedEventArgs e)
        {
             try
            {
            var button = sender as Button; // Cast sender to Button
            var lobbyId = button?.CommandParameter as string; // Get the CommandParameter

            if (lobbyId == null)
            {
                MessageBox.Show("Lobby ID is null.");
                return;
            }

            var content = new StringContent(JsonSerializer.Serialize(new { }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}lobby/join/{lobbyId}", content);
            if (response.IsSuccessStatusCode)
            {
                var lobbyWindow = new LobbyWindow(lobbyId);
                lobbyWindow.Show();

                Close();
            }
            else
            {
                MessageBox.Show("Failed to join lobby.");
            }
            }
            catch (Exception ex)
            {

            }

        }


    }
}
