using BoardGameFrontend.Helpers;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Models;
using System.Windows.Controls;

namespace BoardGameFrontend.ChatLogManager
{
    public class HeroCardChatLogManager
    {
        private ChatGameManager _chat { get; set; }
        private Game _game { get; set; }

        public HeroCardChatLogManager(ChatGameManager chat, Game game)
        {
            _chat = chat;
            _game = game;
        }


        public void PickedCard(HeroCardPicked data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if (player == null) return;

            _chat.AddMessage($"picked card {data.Card.HeroName}.", player);

            if (data.Reward == null) return;

            var rewardString = $"";
            rewardString += RewardDescriptionBuilder.BuildDescription(data.Reward);
            if(rewardString.Length > 0){
                _chat.AddMessage($"{rewardString}.", player);
            }
        }

        public void ReplacedNextHeroCard(ReplaceNextHeroEventData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            if (player == null) return;

            _chat.AddMessage($"replaced next hero card with {data.Hero.HeroName}.", player);
        }

        public void BuffedCard(BuffHeroData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            if (player == null) return;

            var heroCard = player.PlayerHeroCardManager.GetHeroCardById(data.HeroId);
            if (heroCard == null) return;

            _chat.AddMessage($"buffed card {heroCard.HeroName}.", player);
        }


    }
}