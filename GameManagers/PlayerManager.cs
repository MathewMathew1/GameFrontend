using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;

using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    public class PlayerManager : INotifyPropertyChanged
    {
        public ObservableCollection<PlayerInGameViewModel> Players { get; set; } = new ObservableCollection<PlayerInGameViewModel>();
        public ObservableCollection<PlayerInGameViewModel> PlayersBasedOnMorale { get; private set; } = new ObservableCollection<PlayerInGameViewModel>();
        
        private PlayerInGameViewModel _selectedPlayer;
        public PlayerInGameViewModel SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                if (_selectedPlayer != value)
                {
                    _selectedPlayer = value;
                    OnPropertyChanged(nameof(SelectedPlayer));
                }
            }
        } 

        public void SetupMoralesPlayers(List<Player> players){
            PlayersBasedOnMorale.Clear(); 

            foreach(var player in players){
                var playerInGame = GetPlayerById(player.Id);

                if(playerInGame == null) continue;
                PlayersBasedOnMorale.Add(playerInGame);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
        }

        public void EndTurn(Guid playerId)
        {
            
            var player = Players.FirstOrDefault(x => x.Id == playerId);
            if (player == null) return;

            player.ResourceManager.EndOfTurnIncome();
            
            var tempSignets = player.PlayerAuraManager.AurasTypes.Count(aura => aura.Aura == AurasType.TEMPORARY_SIGNET);
            player.ResourceHeroManager.SubtractResource(ResourceHeroType.Signet, tempSignets);
            player.PlayerAuraManager.ResetAura();
            player.ResetStorage();
        }

        public void EndOfRound(){
            foreach(var player in Players){
                player.ResourceManager.EndOfRoundIncome();
            }
        }

        public void GetColorByName(){

        }

        public void SetPlayers(List<PlayerInGameViewModel> newPlayers)
        {
            var colorIndex = 0;
            Players.Clear();
            foreach (var player in newPlayers)
            {
                Players.Add(player);
                PlayersBasedOnMorale.Add(player);
                colorIndex += 1; 
                SubscribeToPlayerMoraleChanges(player);
            }
            OnPropertyChanged(nameof(Players));
        }

        private void SubscribeToPlayerMoraleChanges(PlayerInGameViewModel player)
        {
            // Subscribe to the PropertyChanged event
            player.PropertyChanged += Player_PropertyChanged;
        }

        private void Player_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is PlayerInGameViewModel player && e.PropertyName == nameof(PlayerInGameViewModel.Morale))
            {
                // Perform action when morale changes
                UpdatePlayersBasedOnMorale(player);
            }
        }

        public void AddMoraleToPlayer(PlayerInGameViewModel player, int addedMorale)
        {
            if (addedMorale == 0) return;
            player.Morale += addedMorale;
            UpdatePlayersBasedOnMorale(player);
        }

        private void UpdatePlayersBasedOnMorale(PlayerInGameViewModel? updatedPlayer = null)
        {
            var sortedPlayers = Players
                .OrderByDescending(p => p.Morale)
                .ThenBy(p => p == updatedPlayer ? -1 : 0)
                .ToList();

            PlayersBasedOnMorale.Clear();
            foreach (var player in sortedPlayers)
            {
                PlayersBasedOnMorale.Add(player);
            }

            OnPropertyChanged(nameof(PlayersBasedOnMorale));
        }

        public PlayerInGameViewModel? GetPlayerById(Guid Id)
        {
            return Players.FirstOrDefault(player => player.Id == Id);
        }

        public void ResetAllPlayersPlayedTurn()
        {
            foreach (var player in PlayersBasedOnMorale)
            {
                player.AlreadyPlayedCurrentPhase = false;
            }
        }

    }
}
