using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BoardGameFrontend.Commands;
using BoardGameFrontend.Helpers;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Services
{
    public class ServiceLobbyWindow : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private readonly string _lobbyId;

        private bool _limitedHeroPool;
        public bool LimitedHeroPool
        {
            get => _limitedHeroPool;
            set
            {
                if (_limitedHeroPool != value)
                {
                    _limitedHeroPool = value;
                    OnPropertyChanged(nameof(LimitedHeroPool));
                    UpdateLobbyInfoAsync();
                }
            }
        }

        private bool _sameAmountOfMercenariesEachRound;
        public bool SameAmountOfMercenariesEachRound
        {
            get => _sameAmountOfMercenariesEachRound;
            set
            {
                if (_sameAmountOfMercenariesEachRound != value)
                {
                    _sameAmountOfMercenariesEachRound = value;
                    OnPropertyChanged(nameof(SameAmountOfMercenariesEachRound));
                    UpdateLobbyInfoAsync();
                }
            }
        }

        private bool _moreHeroCardsPerRound;
        public bool MoreHeroCardsPerRound
        {
            get => _moreHeroCardsPerRound;
            set
            {
                if (_moreHeroCardsPerRound != value)
                {
                    _moreHeroCardsPerRound = value;
                    OnPropertyChanged(nameof(MoreHeroCardsPerRound));
                    UpdateLobbyInfoAsync();
                }
            }
        }

        private bool _removePropheciesAtLastRound;
        public bool RemovePropheciesAtLastRound
        {
            get => _removePropheciesAtLastRound;
            set
            {
                if (_removePropheciesAtLastRound != value)
                {
                    _removePropheciesAtLastRound = value;
                    OnPropertyChanged(nameof(RemovePropheciesAtLastRound));
                    UpdateLobbyInfoAsync();
                }
            }
        }

        public ObservableCollection<TurnTypes> TurnTypesData { get; } = new ObservableCollection<TurnTypes>
        {
            TurnTypes.PHASE_BY_PHASE,
            TurnTypes.FULL_TURN
        };

        private TurnTypes _selectedTurnType;

        public TurnTypes SelectedTurnType
        {
            get => _selectedTurnType;
            set
            {
                if (_selectedTurnType != value)
                {
                    _selectedTurnType = value;
                    OnPropertyChanged(nameof(SelectedTurnType));
                    UpdateLobbyInfoAsync();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool UpdateLobbyData = true;

        public void UpdateStartGameData(StartGameModel startGameModel)
        {
            UpdateLobbyData = false;
            LimitedHeroPool = startGameModel.LessCards;
            MoreHeroCardsPerRound = startGameModel.MoreHeroCards;
            SelectedTurnType = startGameModel.TurnType;
            RemovePropheciesAtLastRound = startGameModel.RemovePropheciesAtLastRound;
            UpdateLobbyData = true;
        }

        public ServiceLobbyWindow(HttpClient httpClient, string lobbyId)
        {
            _httpClient = httpClient;
            _lobbyId = lobbyId;

            StartGameCommand = new RelayCommand(async _ => await StartGameAsync());
            LeaveLobbyCommand = new RelayCommand(async _ => await LeaveLobbyAsync());
            DestroyLobbyCommand = new RelayCommand(async _ => await DestroyLobbyAsync());
            EndTurnCommand = new RelayCommand(async _ => await EndTurnAsync());
            EndBoardTurnCommand = new RelayCommand(async _ => await EndBoardTurnAsync());
            GoldIntoMovementCommand = new RelayCommand(async _ => await GoldIntoMovementAsync());
            ConvertMovementsCommand = new RelayCommand(async _ => await ConvertMovementsAsync());
            SkipArtifactPhaseCommand = new RelayCommand(async _ => await SkipArtifact());
            StopSwappingTokensCommand = new RelayCommand(async _ => await EndSwappingTokensAsync());

            BuyMercenaryCommand = new RelayCommandWithTypes<int>(async mercenaryId => await BuyMercenary(mercenaryId));
            TakeRoyalCardCommand = new RelayCommandWithTypes<int>(async mercenaryId => await PickRoyalCard(mercenaryId));
            BanishRoyalCardCommand = new RelayCommandWithTypes<int>(async mercenaryId => await BanishRoyalCard(mercenaryId));
            RerollMercenaryCommand = new RelayCommandWithTypes<int>(async mercenaryId => await RerollMercenary(mercenaryId));
            FulfillMercenaryCommand = new RelayCommandWithTypes<int>(async mercenaryId => await FulfillMercenary(mercenaryId));
            LockMercenaryCommand = new RelayCommandWithTypes<int>(async mercenaryId => await LockMercenary(mercenaryId));
            BuffHeroCommand = new RelayCommandWithTypes<int>(async mercenaryId => await BuffHero(mercenaryId));
            ReplaceHeroCommand = new RelayCommandWithTypes<int>(async mercenaryId => await ReplaceNextHero(mercenaryId));
        }

        private StringStorage _stringStorage = new StringStorage();
        private readonly string START_GAME = "Start game";
        private readonly string LEAVE_LOBBY = "Leave lobby";
        private readonly string DESTROY_LOBBY = "Destroy lobby";

        public ICommand StartGameCommand { get; }
        public ICommand SkipArtifactPhaseCommand { get; }
        public ICommand LeaveLobbyCommand { get; }
        public ICommand DestroyLobbyCommand { get; }
        public ICommand EndTurnCommand { get; }
        public ICommand EndBoardTurnCommand { get; }
        public ICommand GoldIntoMovementCommand { get; }
        public ICommand ConvertMovementsCommand { get; }
        public ICommand StopSwappingTokensCommand { get; }


        public RelayCommandWithTypes<int> BuyMercenaryCommand { get; }
        public RelayCommandWithTypes<int> TakeRoyalCardCommand { get; }
        public RelayCommandWithTypes<int> BanishRoyalCardCommand { get; }
        public RelayCommandWithTypes<int> RerollMercenaryCommand { get; }
        public RelayCommandWithTypes<int> FulfillMercenaryCommand { get; }
        public RelayCommandWithTypes<int> LockMercenaryCommand { get; }
        public RelayCommandWithTypes<int> BuffHeroCommand { get; }
        public RelayCommandWithTypes<int> ReplaceHeroCommand { get; }


        private async Task StartGameAsync()
        {
            await _stringStorage.RunIfNotExistsAsync(START_GAME, async () =>
            {
                try
                {
                    var content = new StringContent(JsonSerializer.Serialize(new { TurnType = SelectedTurnType, LessCards = LimitedHeroPool, MoreHeroCards = MoreHeroCardsPerRound }), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}game/start/{_lobbyId}", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Failed to start lobby.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            });
        }

        private async Task LeaveLobbyAsync()
        {
            await _stringStorage.RunIfNotExistsAsync(LEAVE_LOBBY, async () =>
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"{UrlHelper.GetBaseUrl()}lobby/leave/{_lobbyId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var mainWindow = new MainViewWindow();
                        mainWindow.Show();

                    }
                    else
                    {
                        MessageBox.Show("Failed to leave lobby.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            });
        }

        private async Task DestroyLobbyAsync()
        {
            await _stringStorage.RunIfNotExistsAsync(DESTROY_LOBBY, async () =>
           {
               try
               {
                   var response = await _httpClient.DeleteAsync($"{UrlHelper.GetBaseUrl()}lobby/destroy/{_lobbyId}");
                   if (!response.IsSuccessStatusCode)
                   {
                       MessageBox.Show("Failed to destroy lobby.");
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"An error occurred: {ex.Message}");
                   MessageBox.Show($"An error occurred: {ex.Message}");
               }
           });
        }

        private async Task EndTurnAsync()
        {
            var response = await _httpClient.GetAsync($"{UrlHelper.GetBaseUrl()}mercenary/end/{_lobbyId}");
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("End turn.");
            }
        }

        public async Task<bool> MoveToTile(int tileId, bool isFullMovement, bool AdjacentMovement, int? TeleportationPlace = null)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(new { TileId = tileId, FullMovement = isFullMovement, TeleportationPlace, AdjacentMovement = AdjacentMovement }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}board/moveToTile/{_lobbyId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Unable to move to tile.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> TeleportToTile(int tileId)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(new { TileId = tileId }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}board/teleport/{_lobbyId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Unable to teleport to tile.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> RotatePawn(int tileId)
        {

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(new { TileId = tileId }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}board/rotate/{_lobbyId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Unable to rotate pawn.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }

        }

        public async Task BlockTile(int tileId)
        {
            await _stringStorage.RunIfNotExistsAsync("BlockTile", async () =>
           {
               try
               {
                   var content = new StringContent(JsonSerializer.Serialize(new { TileId = tileId }), Encoding.UTF8, "application/json");

                   var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}board/block/{_lobbyId}", content);
                   if (!response.IsSuccessStatusCode)
                   {
                       MessageBox.Show("Unable to block tile.");
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"An error occurred: {ex.Message}");
                   MessageBox.Show($"An error occurred: {ex.Message}");
               }
           });
        }

        public async Task PlayArtifact(int artifactId, bool isFirstEffect, bool replayArtifact)
        {
            await _stringStorage.RunIfNotExistsAsync("Play Artifact", async () =>
           {
               try
               {
                   var content = new StringContent(JsonSerializer.Serialize(new { ArtifactId = artifactId, IsFirstEffect = isFirstEffect, ReplayArtifact = replayArtifact }), Encoding.UTF8, "application/json");

                   var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}artifact/play/{_lobbyId}", content);
                   if (!response.IsSuccessStatusCode)
                   {
                       MessageBox.Show("Unable to play artifact.");
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"An error occurred: {ex.Message}");
                   MessageBox.Show($"An error occurred: {ex.Message}");
               }
           });
        }

        public async Task SkipArtifact()
        {
            await _stringStorage.RunIfNotExistsAsync("Skip Artifact Phase", async () =>
           {
               try
               {
                   var response = await _httpClient.GetAsync($"{UrlHelper.GetBaseUrl()}artifact/skip/{_lobbyId}");
                   if (!response.IsSuccessStatusCode)
                   {
                       MessageBox.Show("Unable to skip artifact phase.");
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"An error occurred: {ex.Message}");
                   MessageBox.Show($"An error occurred: {ex.Message}");
               }
           });
        }



        public async Task RerollArtifact(int artifactId)
        {
            await _stringStorage.RunIfNotExistsAsync("Reroll Artifact", async () =>
            {
                try
                {
                    var content = new StringContent(JsonSerializer.Serialize(new { ArtifactId = artifactId }), Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}artifact/reroll/{_lobbyId}", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Unable to play artifact.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            });
        }

        public async Task BuyMercenary(int mercenaryId)
        {
            await _stringStorage.RunIfNotExistsAsync("Buy mercenary", async () =>
           {
               try
               {
                   var content = new StringContent(JsonSerializer.Serialize(new { MercenaryId = mercenaryId }), Encoding.UTF8, "application/json");

                   var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}mercenary/buy/{_lobbyId}", content);
                   if (!response.IsSuccessStatusCode)
                   {
                       MessageBox.Show("Failed to buy mercenary.");
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"An error occurred: {ex.Message}");
                   MessageBox.Show($"An error occurred: {ex.Message}");
               }
           });
        }

        public async Task BanishRoyalCard(int royalCardId)
        {
            await _stringStorage.RunIfNotExistsAsync("Banish Royal Card", async () =>
            {
                try
                {
                    var content = new StringContent(JsonSerializer.Serialize(new { RoyalCardId = royalCardId }), Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}royalCard/banish/{_lobbyId}", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Failed to banish royal card.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            });
        }

        public async Task PickRoyalCard(int royalCardId)
        {
            await _stringStorage.RunIfNotExistsAsync("Pick Royal Card", async () =>
            {
                try
                {
                    var content = new StringContent(JsonSerializer.Serialize(new { RoyalCardId = royalCardId }), Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}royalCard/take/{_lobbyId}", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Failed to take royal card.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            });
        }

        public async Task TakeArtifacts(List<int> artifactsIds)
        {
            await _stringStorage.RunIfNotExistsAsync("Take Artifact", async () =>
           {
               try
               {
                   var content = new StringContent(JsonSerializer.Serialize(new { ArtifactsIds = artifactsIds }), Encoding.UTF8, "application/json");

                   var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}artifact/take/{_lobbyId}", content);
                   if (!response.IsSuccessStatusCode)
                   {
                       MessageBox.Show("Failed to take artifacts.");
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"An error occurred: {ex.Message}");
                   MessageBox.Show($"An error occurred: {ex.Message}");
               }
           });
        }

        public async Task RerollMercenary(int mercenaryId)
        {
            await _stringStorage.RunIfNotExistsAsync("Reroll Mercenary", async () =>
           {
               try
               {
                   var content = new StringContent(JsonSerializer.Serialize(new { MercenaryId = mercenaryId }), Encoding.UTF8, "application/json");

                   var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}mercenary/reroll/{_lobbyId}", content);
                   if (!response.IsSuccessStatusCode)
                   {
                       MessageBox.Show("Failed to reroll mercenary.");
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"An error occurred: {ex.Message}");
                   MessageBox.Show($"An error occurred: {ex.Message}");
               }
           });
        }

        public async Task FulfillMercenary(int mercenaryId)
        {
            await _stringStorage.RunIfNotExistsAsync("Fufill Mercenary", async () =>
           {
               try
               {
                   var content = new StringContent(JsonSerializer.Serialize(new { MercenaryId = mercenaryId }), Encoding.UTF8, "application/json");

                   var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}mercenary/fulfill/{_lobbyId}", content);
                   if (!response.IsSuccessStatusCode)
                   {
                       MessageBox.Show("Failed to fulfill mercenary prophecy.");
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"An error occurred: {ex.Message}");
                   MessageBox.Show($"An error occurred: {ex.Message}");
               }
           });
        }

        public async Task LockMercenary(int mercenaryId)
        {
            await _stringStorage.RunIfNotExistsAsync("Lock Mercenary", async () =>
           {
               try
               {
                   var content = new StringContent(JsonSerializer.Serialize(new { MercenaryId = mercenaryId }), Encoding.UTF8, "application/json");

                   var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}mercenary/lock/{_lobbyId}", content);
                   if (!response.IsSuccessStatusCode)
                   {
                       MessageBox.Show("Failed to lock mercenary.");
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"An error occurred: {ex.Message}");
                   MessageBox.Show($"An error occurred: {ex.Message}");
               }
           });
        }

        public async Task BuffHero(int heroId)
        {
            await _stringStorage.RunIfNotExistsAsync("BuffHero", async () =>
           {
               try
               {
                   var content = new StringContent(JsonSerializer.Serialize(new { HeroCardId = heroId }), Encoding.UTF8, "application/json");

                   var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}heroCard/buff/{_lobbyId}", content);
                   if (!response.IsSuccessStatusCode)
                   {
                       MessageBox.Show("Failed to buff hero.");

                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"An error occurred: {ex.Message}");
                   MessageBox.Show($"An error occurred: {ex.Message}");
               }
           });
        }

        public async Task ReplaceNextHero(int heroId)
        {
            await _stringStorage.RunIfNotExistsAsync("Replace next hero", async () =>
            {
                try
                {
                    var content = new StringContent(JsonSerializer.Serialize(new { HeroCardId = heroId }), Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}heroCard/replaceNext/{_lobbyId}", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Failed to replace next hero card.");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            });
        }

        private async Task EndBoardTurnAsync()
        {
            await _stringStorage.RunIfNotExistsAsync("End Board Turn", async () =>
            {
                var response = await _httpClient.GetAsync($"{UrlHelper.GetBaseUrl()}board/end/{_lobbyId}");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to end turn.");
                }
                else
                {
                    if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;
                    var game = window.Game;
                    game.PawnManager.CancelMovement();
                }
            });
        }

        private async Task EndSwappingTokensAsync()
        {
            await _stringStorage.RunIfNotExistsAsync("EndSwappingTokens", async () =>
            {
                var response = await _httpClient.GetAsync($"{UrlHelper.GetBaseUrl()}board/stopSwapping/{_lobbyId}");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to end swapping tokens.");
                }
                else
                {
                    if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;
                    var game = window.Game;
                    game.PawnManager.CancelMovement();
                }
            });
        }

        private async Task GoldIntoMovementAsync()
        {
            await _stringStorage.RunIfNotExistsAsync("Gold into movement", async () =>
            {
                var response = await _httpClient.GetAsync($"{UrlHelper.GetBaseUrl()}board/goldIntoMovement/{_lobbyId}");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to convert gold into movement");
                }
                else
                {
                    if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;
                    var game = window.Game;
                    game.PawnManager.CancelMovement();
                }
            });
        }

        private async Task ConvertMovementsAsync()
        {
            await _stringStorage.RunIfNotExistsAsync("ConvertMovement", async () =>
            {
                var response = await _httpClient.GetAsync($"{UrlHelper.GetBaseUrl()}board/convertMovement/{_lobbyId}");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to convert full movement into empty");
                }
                else
                {
                    if (!(Application.Current.MainWindow is LobbyWindow window && window.Game != null)) return;
                    var game = window.Game;
                    game.PawnManager.CancelMovement();
                }
            });
        }

        public async Task SwapToken(int tileIdOne, int tileIdTwo)
        {
            await _stringStorage.RunIfNotExistsAsync("SwapTokens", async () =>
            {
                var content = new StringContent(JsonSerializer.Serialize(new { TileIdOne = tileIdOne, tileIdTwo = tileIdTwo }), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}board/swap/{_lobbyId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Failed to swap tokens");
                }

            });
        }

        private async Task UpdateLobbyInfoAsync()
        {
            await _stringStorage.RunIfNotExistsAsync("Update Lobby Info", async () =>
            {
                try
                {
                    if (UpdateLobbyData == false) return;
                    var content = new StringContent(JsonSerializer.Serialize(new StartGameModel
                    {
                        LessCards = LimitedHeroPool,
                        RemovePropheciesAtLastRound = RemovePropheciesAtLastRound,
                        MoreHeroCards = MoreHeroCardsPerRound,
                        TurnType = SelectedTurnType,
                        SameAmountOfMercenariesEachRound = SameAmountOfMercenariesEachRound
                    }), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PatchAsync($"{UrlHelper.GetBaseUrl()}lobby/updateInfo/{_lobbyId}", content);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Unable to update lobby data.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            });
        }


    }
}