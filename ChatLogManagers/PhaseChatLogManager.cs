using BoardGameFrontend.Managers;
using BoardGameFrontend.Models;
using System.Windows.Controls;

namespace BoardGameFrontend.ChatLogManager
{
    public class PhaseChatLogManager
    {
        private ChatGameManager _chat { get; set; }
        private Game _game { get; set; }

        public PhaseChatLogManager(ChatGameManager chat, Game game)
        {
            _chat = chat;
            _game = game;
        }

        public void StartTurnData(StartTurnData data)
        {
            if (data.NewCards.Count > 0)
            {
                _chat.AddMessage($"{data.NewCards.Count} new hero cards have been added.");
            }
        }

        public void ArtifactPhaseStarted()
        {
            _chat.AddMessage($"Artifact picking phase has started.");
        }

        public void BoardPhaseStarted()
        {
            _chat.AddMessage($"Board phase started.");
        }

        public void NewPlayerTurn(PlayerViewModel player)
        {
            var _player = _game.PlayerManager.GetPlayerById(player.Id);
            if(_player == null) return;

            _chat.AddMessage($"turn is on.", _player);
        }

        public void HeroTurnEnded(PlayerViewModel player)
        {
            var _player = _game.PlayerManager.GetPlayerById(player.Id);
            if(_player == null) return;

            _chat.AddMessage($"ended hero turn.", _player);
        }

        public void EndOfTurn()
        {
            _chat.AddMessage($"Turn has ended.");
        }


        public void EndOfTurn(EndOfPlayerTurnData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"has ended his turn.", player);
        }

        public void EndOfRound()
        {
            _chat.AddMessage($"Round has ended.");
        }

        public void HeroCardPickingPhaseStarted()
        {
            _chat.AddMessage($"Hero card picking phase has started.");
        }

        public void MercenaryPhaseStarted()
        {
            _chat.AddMessage($"Mercenary picking phase has started.");
        }


    }
}