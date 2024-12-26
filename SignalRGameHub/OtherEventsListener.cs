using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Models;
using BoardGameFrontend.Managers;
using BoardGameFrontend.ChatLogManager;
using System.Windows.Threading;
using BoardGameFrontend.Windows;
using System.Collections.Generic;
using System;



namespace BoardGameFrontend.SignalHub
{
    public class OtherEventsListener: IDisposable
    {
        private bool _disposed;
        private readonly HubConnection _connection;

        public OtherEventsListener(HubConnection connection, Game game, MainChatLogManager mainChatLogManager, Dispatcher dispatcher, LobbyViewModelManager lobbyVm)
        {
             _connection = connection;
            _disposed = false;

            connection.On<TeleportationData>("TeleportationEvent", (TeleportationData) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.Teleport(TeleportationData);
                    mainChatLogManager.OthersChatLogManager.TeleportationEvent(TeleportationData);
                });
            });

            connection.On<AddAura>("AddAura", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.AddAura(data);
                    mainChatLogManager.OthersChatLogManager.AuraAdded(data);
                });
            });



            connection.On<EndOfGame>("EndOfGame", (EndGameData) =>
            {
                dispatcher.Invoke(() =>
                {
                    // Convert EndOfGame to EndOfGameWithPlayers
                    var endGameWithPlayers = Convert(EndGameData, game, lobbyVm);
                    
                    ShowEndOfGameScore(endGameWithPlayers);
                    mainChatLogManager.OthersChatLogManager.EndOfGame();

                });
            });
            
            connection.On<NewTokensSetupEventData>("NewTokensSetup", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.NewTokens(data);

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
                _connection.Remove("TeleportationEvent");
                _connection.Remove("AddAura");
                _connection.Remove("EndOfGame");
                _connection.Remove("NewTokensSetup");
            }

            _disposed = true;
        }

        public EndOfGameWithPlayers Convert(EndOfGame endGameData, Game game, LobbyViewModelManager lobbyVm)
        {
            var endGameWithPlayers = new EndOfGameWithPlayers
            {
                PlayerScores = new List<ScorePointsTableWithPlayer>(),
                GameTimeSpan = endGameData.GameTimeSpan,
                PlayerTimeSpan = endGameData.PlayerTimeSpan,
            };

            foreach (var playerScore in endGameData.PlayerScores)
            {
                // Assuming you have a method to get PlayerViewModelData by player ID
                var playerViewModelData = game.PlayerManager.GetPlayerById(playerScore.Key);

                if(playerViewModelData == null) continue;

                // Create the ScorePointsTableWithPlayer instance
                var scoreTableWithPlayer = new ScorePointsTableWithPlayer
                {
                    PlayerColor = lobbyVm.GetColorForPlayer(playerViewModelData.Id),
                    Player = playerViewModelData!, // This should be the player's data
                    MoralePoints = playerScore.Value.MoralePoints,
                    SiegePoints = playerScore.Value.SiegePoints,
                    ArmyPoints = playerScore.Value.ArmyPoints,
                    MagicPoints = playerScore.Value.MagicPoints,
                    MercenaryPoints = playerScore.Value.MercenaryPoints,
                    OraclePoints = playerScore.Value.OraclePoints,
                    HeroPoints = playerScore.Value.HeroPoints,
                    ArtefactPoints = playerScore.Value.ArtefactPoints,
                    TokenPoints = playerScore.Value.TokenPoints,
                    RoyalCardPoints = playerScore.Value.RoyalCardPoints,
                    PointsOverall = playerScore.Value.PointsOverall
                };

                endGameWithPlayers.PlayerScores.Add(scoreTableWithPlayer);
            }

            return endGameWithPlayers;
        }

        public void ShowEndOfGameScore(EndOfGameWithPlayers endGameData)
        {
            var scoreWindow = new ScoreWindow(endGameData);

            scoreWindow.DataContext = this;

            scoreWindow.Show();
        }
    }
}
