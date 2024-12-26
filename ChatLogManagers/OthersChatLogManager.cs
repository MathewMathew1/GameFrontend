using BoardGameFrontend.Managers;
using BoardGameFrontend.Models;
using System.Windows.Controls;

namespace BoardGameFrontend.ChatLogManager
{
    public class OthersChatLogManager
    {
        private ChatGameManager _chat { get; set; }
        private Game _game { get; set; }

        public OthersChatLogManager(ChatGameManager chat, Game game)
        {
            _chat = chat;
            _game = game;
        }

        public void TeleportationEvent(TeleportationData data)
        {      
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;
            _chat.AddMessage($"teleported pawn to tile with id ${data.TileId}.", player);
        }

        public void AuraAdded(AddAura data)
        {   
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            if(player == null) return;

            _chat.AddMessage($"$received aura ${data.Aura.Aura}.", player);
        }

        public void EndOfGame()
        {   
            _chat.AddMessage($"Game has ended, Tousont will be free again.");
        }
    }
}