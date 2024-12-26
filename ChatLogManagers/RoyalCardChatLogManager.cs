using BoardGameFrontend.Helpers;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Models;
using System.Windows.Controls;

namespace BoardGameFrontend.ChatLogManager
{
    public class RoyalCardChatLogManager
    {
        private ChatGameManager _chat { get; set; }
        private Game _game { get; set; }

        public RoyalCardChatLogManager(ChatGameManager chat, Game game)
        {
            _chat = chat;
            _game = game;
        }

        public void RoyalCardPlayed(RoyalCardPlayed data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            
            if(player == null) return;
            _chat.AddMessage($"played {data.RoyalCard.Type}.", player);
            var rewardString = $"";
            if(data.Reward != null){
                rewardString += RewardDescriptionBuilder.BuildDescription(data.Reward);
            }
            
            if(rewardString.Length > 0){
                _chat.AddMessage($"{rewardString}.", player);
            }
        }

        public void RoyalCardBanished(BanishRoyalCardEventData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            
            if(player == null) return;
            _chat.AddMessage($"banished {data.RoyalCard.Type}.", player);
        }


    }
}