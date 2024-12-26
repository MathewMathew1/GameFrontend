using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Models;
using BoardGameFrontend.Managers;
using BoardGameFrontend.ChatLogManager;
using System.Windows.Threading;
using System;

namespace BoardGameFrontend.SignalHub
{
    public class MiniPhasesListener: IDisposable
    {
        private bool _disposed;
        private readonly HubConnection _connection;
        public MiniPhasesListener(HubConnection connection, Game game, MainChatLogManager mainChatLogManager, Dispatcher dispatcher, TabManager tabManager)
        {
            _connection = connection;
            _disposed = false;

            connection.On("TeleportationMiniPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.TeleportationPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.TeleportationMiniPhaseStarted();
                });
            });

            connection.On("ArtifactPickMiniPhase", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Artifacts);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.ArtifactPickPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.ArtifactPickMiniPhase();
                });
            });

            connection.On("ArtifactPickMiniPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Artifacts);
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.ArtifactPickMiniPhaseEnded();
                });
            });

            connection.On("FulfillProphecyMiniPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.YourCards);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.FulfilProphecyPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.FulfillProphecyMiniPhaseStarted();
                });
            });

            connection.On("FulfillProphecyMiniPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.FulfillProphecyMiniPhaseEnded();
                });
            });

            connection.On("LockCardMiniPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Mercenaries);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.LockMercenaryPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.LockCardMiniPhaseStarted();
                });
            });

            connection.On("LockCardMiniPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    if(game.PhaseManager.CurrentPhase.Name == PhaseType.BoardPhase){
                        tabManager.SetTab(Tab.Board);
                    }else{
                        tabManager.SetTab(Tab.Mercenaries);
                    }
                    
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.LockCardMiniPhaseEnded();
                });
            });

            connection.On("BuffHeroPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.BuffHeroPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.BuffHeroMiniPhaseStarted();
                });
            });

            connection.On("BuffHeroMiniPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.BuffHeroMiniPhaseEnded();
                });
            });

            connection.On("BlockTileMiniPhaseStarted", (MiniPhaseDataWithDifferentPlayer data) =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.UpdateCurrentPlayer(data.PlayerId, false);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.BlockTilePhase });
                    mainChatLogManager.MiniPhaseChatLogManager.BlockTileMiniPhaseStarted();
                });
            });

            connection.On("BlockTileMiniEnded", (MiniPhaseDataWithDifferentPlayer data) =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.UpdateCurrentPlayer(data.PlayerId, false);
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.BlockTileMiniPhaseEnded();
                });
            });

            connection.On("RoyalMiniPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.RoyalCard);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.RoyalCardPickMiniPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.RoyalCardPickMiniPhaseStarted();
                });
            });

            connection.On("RoyalMiniPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.RoyalCardPickMiniPhaseEnded();
                });
            });

            connection.On("RerollMercenaryMiniPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Mercenaries);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.MercenaryRerollPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.MercenaryRerollMiniPhaseStarted();
                });
            });

            connection.On("ReplayArtifactPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.YourCards);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.ReplayArtifactMiniPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.ArtifactReplayMiniPhaseStarted();
                });
            });

            connection.On("ReplayArtifactPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.YourCards);
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.ArtifactReplayMiniPhaseEnded();
                });
            });   

            connection.On("ReplayArtifactPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.YourCards);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.ReplayArtifactMiniPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.ArtifactReplayMiniPhaseStarted();
                });
            });

            connection.On("ReplaceHeroMiniPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.ReplaceHeroMiniPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.ReplaceNextHeroMiniPhaseStarted();
                });
            });

            connection.On("ReplaceHeroMiniPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.ReplaceNextHeroMiniPhaseEnded();
                });
            });

            connection.On("BanishCarMiniPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.RoyalCard);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.BanishRoyalCard });
                    mainChatLogManager.MiniPhaseChatLogManager.BanishRoyalCardMiniPhaseStarted();
                });
            });

            connection.On("BanishCarMiniPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.BanishRoyalCardMiniPhaseEnded();
                });
            });

            connection.On("SwapTokenMiniPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.SwapTokens });
                    mainChatLogManager.MiniPhaseChatLogManager.SwapTokensMiniPhaseStarted();
                });
            });

            connection.On("SwapTokenMiniPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.SwapTokensMiniPhaseEnded();
                });
            });

            connection.On("RotatePawnMiniPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.Board);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.RotatePawnMiniPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.RotatePawnMiniPhaseStarted();
                    game.GameVisualManager.TilesBorderManager.SetRotateConnections();
                });
            });

            connection.On("RotatePawnMiniPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.HeroCards);
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.RotatePawnMiniPhaseEnded();
                    game.GameVisualManager.TilesBorderManager.RemoveAvailableConnections();
                });
            });


            connection.On("ReplaceHeroToBuyMiniPhaseStarted", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.HeroCards);
                    game.MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.ReplaceHeroToBuyMiniPhase });
                    mainChatLogManager.MiniPhaseChatLogManager.ReplaceNextHeroMiniPhaseStarted();
                });
            });


            connection.On("ReplaceHeroToBuyMiniPhaseEnded", () =>
            {
                dispatcher.Invoke(() =>
                {
                    tabManager.SetTab(Tab.HeroCards);
                    game.MiniPhaseManager.SetCurrentPhase(null);
                    mainChatLogManager.MiniPhaseChatLogManager.ReplaceNextHeroMiniPhaseEnded();
                });
            });


        }

        public void Dispose()
        {
            _connection.Remove("TeleportationMiniPhaseStarted");
            _connection.Remove("ArtifactPickMiniPhase");
            _connection.Remove("ArtifactPickMiniPhaseEnded");
            _connection.Remove("FulfillProphecyMiniPhaseStarted");
            _connection.Remove("FulfillProphecyMiniPhaseEnded");
            _connection.Remove("LockCardMiniPhaseStarted");
            _connection.Remove("LockCardMiniPhaseEnded");
            _connection.Remove("BuffHeroPhaseStarted");
            _connection.Remove("BuffHeroMiniPhaseEnded");
            _connection.Remove("BlockTileMiniPhaseStarted");
            _connection.Remove("BlockTileMiniEnded");
            _connection.Remove("RoyalMiniPhaseStarted");
            _connection.Remove("RoyalMiniPhaseEnded");
            _connection.Remove("RerollMercenaryMiniPhaseStarted");
            _connection.Remove("ReplayArtifactPhaseStarted");
            _connection.Remove("ReplayArtifactPhaseEnded");
            _connection.Remove("ReplaceHeroMiniPhaseStarted");
            _connection.Remove("ReplaceHeroMiniPhaseEnded");
            _connection.Remove("BanishCarMiniPhaseStarted");
            _connection.Remove("BanishCarMiniPhaseEnded");
            _connection.Remove("SwapTokenMiniPhaseStarted");
            _connection.Remove("SwapTokenMiniPhaseEnded");
            _connection.Remove("RotatePawnMiniPhaseStarted");
            _connection.Remove("RotatePawnMiniPhaseEnded");
        }
    }
}