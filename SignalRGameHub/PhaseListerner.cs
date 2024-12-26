using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Models;
using BoardGameFrontend.Managers;
using System.Windows.Threading;
using BoardGameFrontend.ChatLogManager;
using System;


namespace BoardGameFrontend.SignalHub
{
    public class PhasesListener: IDisposable
    {
        private bool _disposed;
        private readonly HubConnection _connection;

        public PhasesListener(HubConnection connection, Game game, MainChatLogManager mainChatLogManager, Dispatcher dispatcher, TabManager tabManager)
        {
             _connection = connection;
            _disposed = false;

            connection.On<DummyPhaseStarted>("ArtifactPhaseStarted", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.UpdateCurrentPlayer(data.Player.Id);
                    tabManager.SetTab(Tab.Artifacts);
                    game.PhaseManager.SetCurrentPhase(new Phase { Name = PhaseType.ArtifactPhase });

                    mainChatLogManager.PhaseChatLogManager.ArtifactPhaseStarted();
                });
            });

            connection.On<StartTurnData>("NewCardsSetup", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.UpdateCurrentPlayer(data.Player.Id);
                    game.UpdateCurrentTurn(data.TurnCount, data.RoundCount);

                    if (data.NewCards.Count > 0)
                    {
                        game.HeroCardsBoardManager.SetHeroCards(data.NewCards);
                    }

                    tabManager.SetTab(Tab.Artifacts);
                    game.PhaseManager.SetCurrentPhase(new Phase { Name = PhaseType.ArtifactPhase });

                    mainChatLogManager.PhaseChatLogManager.StartTurnData(data);
                });
            });

            connection.On<PlayerViewModel>("BoardPhaseStarted", (player) =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.UpdateCurrentPlayer(player.Id);
                    game.PhaseManager.SetCurrentPhase(new Phase { Name = PhaseType.BoardPhase });
                    mainChatLogManager.PhaseChatLogManager.BoardPhaseStarted();
                });
            });

            connection.On<PlayerViewModel>("MercenaryPhaseStarted", (player) =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Mercenaries);
                    game.UpdateCurrentPlayer(player.Id);
                    game.PhaseManager.SetCurrentPhase(new Phase { Name = PhaseType.MercenaryPhase });
                    mainChatLogManager.PhaseChatLogManager.MercenaryPhaseStarted();
                });
            });

            connection.On<PlayerViewModel>("NewPlayerTurn", (player) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.UpdateCurrentPlayer(player.Id, false);
                    mainChatLogManager.PhaseChatLogManager.NewPlayerTurn(player);
                });
            });

            connection.On<PlayerViewModel>("HeroTurnEnded", (player) =>
            {
                dispatcher.Invoke(() =>
                {
                    var playerInGame = game.PlayerManager.GetPlayerById(player.Id);
                    if(playerInGame==null) return;
                    
                    playerInGame.ResetCurrentHeroCard();
                    mainChatLogManager.PhaseChatLogManager.HeroTurnEnded(player);

                });
            });

            connection.On<EndOfTurnEventData>("EndOfTurn", (data) =>
           {
               dispatcher.Invoke(() =>
               {
                   game.EndOfTurn(data);
                   mainChatLogManager.PhaseChatLogManager.EndOfTurn();
               });
           });

            connection.On("EndOfPlayerTurn", (EndOfPlayerTurnData data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.EndOfTurn(data.Player.Id);
                    mainChatLogManager.PhaseChatLogManager.EndOfTurn(data);
                });
            });

            connection.On("EndOfRound", (EndOfRoundData endOfRoundData) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.EndOfRound(endOfRoundData);
                    mainChatLogManager.PhaseChatLogManager.EndOfRound();
                });
            });

            connection.On<DummyPhaseStarted>("HeroCardPickingPhaseStarted", (data) =>
           {
               dispatcher.Invoke(() =>
               {
                   tabManager.SetTab(Tab.HeroCards);
                   game.UpdateCurrentPlayer(data.Player.Id);
                   game.PhaseManager.SetCurrentPhase(new Phase { Name = PhaseType.HeroCardPickingPhase });
                   mainChatLogManager.PhaseChatLogManager.HeroCardPickingPhaseStarted();
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
                _connection.Remove("HeroCardPickingPhaseStarted");
                _connection.Remove("EndOfRound");
                _connection.Remove("EndOfPlayerTurn");
                _connection.Remove("HeroTurnEnded");
                _connection.Remove("ArtifactPhaseStarted");
                _connection.Remove("NewCardsSetup");
                _connection.Remove("BoardPhaseStarted");
                _connection.Remove("MercenaryPhaseStarted");
                _connection.Remove("NewPlayerTurn");    
            }

            _disposed = true;
        }
    }
}