using Microsoft.AspNetCore.SignalR.Client;
using BoardGameFrontend.Models;
using BoardGameFrontend.Managers;
using System;
using System.Windows.Threading;
using System.Linq;
using BoardGameFrontend.ChatLogManager;
using BoardGameFrontend.AutoMapper;

namespace BoardGameFrontend.SignalHub
{
    public class HeroCardsListener : IDisposable
    {
        private readonly HubConnection _connection;
        private bool _disposed;

        public HeroCardsListener(HubConnection connection, Game game, MainChatLogManager mainChatLogManager, Dispatcher dispatcher)
        {
            _connection = connection;
            _disposed = false;

            _connection.On<HeroCardPicked>("CardPicked", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    var cardId = data.CurrentHeroCard.ReplacedHeroCard != null ? data.CurrentHeroCard.ReplacedHeroCard.HeroCard.Id : data.Card.Id;
                    var cardPicked = game.HeroCardsBoardManager.SetCardAsPickedByUser(cardId, data.Player);
                    if (cardPicked != null)
                    {
                        var playerInGame = game.PlayerManager.Players.First(p => p.Id == data.Player.Id);
                        bool isLeft = cardPicked.LeftSide.Id == data.Card.Id;

                        var currentHeroCard = AutoMapperConfig.Mapper.Map<CurrentHeroCard>(data.CurrentHeroCard);
                        if (data.CurrentHeroCard.ReplacedHeroCard != null)
                        {
                            playerInGame.PlayerHeroCardManager.RemoveHeroCardById(data.CurrentHeroCard.HeroCard.Id);
                        }
                        if (isLeft)
                        {
                            playerInGame.SetCurrentHeroCard(currentHeroCard, cardPicked.LeftSide, isLeft);
                        }
                        else
                        {
                            playerInGame.SetCurrentHeroCard(currentHeroCard, cardPicked.RightSide, isLeft);
                        }
                    }
                    if (data.Reward != null)
                    {
                        var player = game.PlayerManager.GetPlayerById(data.Player.Id);
                        if (player != null)
                        {
                            player.ReceiveRewards(data.Reward);
                        }
                    }
                    mainChatLogManager.HeroCardChatLogManager.PickedCard(data);
                });
            });

            _connection.On<BuffHeroData>("HeroCardBuffed", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.BuffHero(data);
                    mainChatLogManager.HeroCardChatLogManager.BuffedCard(data);
                });
            });

            _connection.On<ReplaceNextHeroEventData>("HeroCardReplaced", (data) =>
            {
                dispatcher.Invoke(() =>
                {
                    game.ReplacedNextHeroCardPlayed(data);
                    mainChatLogManager.HeroCardChatLogManager.ReplacedNextHeroCard(data);
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
                // Unsubscribe from SignalR events to prevent memory leaks
                _connection.Remove("CardPicked");
                _connection.Remove("HeroCardBuffed");
                _connection.Remove("HeroCardReplaced");
            }

            _disposed = true;
        }
    }
}
