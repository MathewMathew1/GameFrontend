using BoardGameFrontend.Helpers;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Models;
using System.Windows.Controls;

namespace BoardGameFrontend.ChatLogManager
{
    public class MercenaryChatLogManager
    {
        private ChatGameManager _chat { get; set; }
        private Game _game { get; set; }

        public MercenaryChatLogManager(ChatGameManager chat, Game game)
        {
            _chat = chat;
            _game = game;
        }

        public void MercenaryPicked(MercenaryPickedData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;
            

            _chat.AddMessage($"picked mercenary {data.Card.NameEng}.", player);

            if(data.Reward == null) return;
            
            var rewardString = $"";
            rewardString += RewardDescriptionBuilder.BuildDescription(data.Reward);
            if(rewardString.Length > 0){
                _chat.AddMessage($"{rewardString}.", player);
            }
            
        }

        public void MercenaryRerolled(MercenaryRerolledData data)
        {
            var player = _game.CurrentPlayer;
            if(player == null) return;
            
            _chat.AddMessage($"rerolled mercenary {data.Card.NameEng}.", player);
        }

  
        public void BuyableMercenariesRefreshed()
        {
            _chat.AddMessage($"Buyable mercenaries have been updated");
        }

        public void FulfillProphecy(FulfillProphecy data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            if(player == null) return;
            
            var mercenary = player.PlayerMercenariesManager.GetMercenaryById(data.MercenaryId);
            if(mercenary == null) return;

            _chat.AddMessage($"has fulfilled prophecy on {mercenary.NameEng}", player);
        }
       
        public void LockMercenary(LockMercenaryData data)
        {
            var player = _game.CurrentPlayer;
            if(player == null) return;
            
            var mercenary = player.PlayerMercenariesManager.GetMercenaryById(data.MercenaryId);
            if(mercenary == null) return;

            _chat.AddMessage($"has locked mercenary {mercenary.NameEng} for others to buy.", player);
        }





    }
}