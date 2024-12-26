using BoardGameFrontend.Managers;
using System;
using System.Windows.Controls;

namespace BoardGameFrontend.ChatLogManager
{
    public class MiniPhaseChatLogManager
    {
        private ChatGameManager _chat { get; set; }
        private Game _game { get; set; }
        private Random _random = new Random();

        public MiniPhaseChatLogManager(ChatGameManager chat, Game game)
        {
            _chat = chat;
            _game = game;
        }

        public void TeleportationMiniPhaseStarted()
        {
        
            _chat.AddMessage($"is teleporting pawn", _game.CurrentPlayer);
        }

        public void ArtifactPickMiniPhase()
        {
            _chat.AddMessage($"is picking artifacts", _game.CurrentPlayer);
        }

        public void BanishRoyalCardMiniPhaseStarted()
        {
            _chat.AddMessage($"is banishing royal card", _game.CurrentPlayer);
        }

        public void BanishRoyalCardMiniPhaseEnded()
        {
            _chat.AddMessage($"stopped banishing royal card", _game.CurrentPlayer);
        }

        public void SwapTokensMiniPhaseStarted()
        {
            _chat.AddMessage($"started swapping tokens", _game.CurrentPlayer);
        }

        public void SwapTokensMiniPhaseEnded()
        {
            _chat.AddMessage($"stopped swapping tokens", _game.CurrentPlayer);
        }

        public void ArtifactPickMiniPhaseEnded()
        {
            _chat.AddMessage($"stopped picking artifacts", _game.CurrentPlayer);
        }

        public void RoyalCardPickMiniPhaseStarted()
        {
            _chat.AddMessage($"started picking Royal Card", _game.CurrentPlayer);
        }

        public void RoyalCardPickMiniPhaseEnded()
        {
            _chat.AddMessage($"stopped picking Royal Card", _game.CurrentPlayer);
        }

         public void ReplaceNextHeroMiniPhaseStarted()
        {
            _chat.AddMessage($"started replacing next hero", _game.CurrentPlayer);
        }

        public void ReplaceNextHeroMiniPhaseEnded()
        {
            _chat.AddMessage($"stopped replacing next hero", _game.CurrentPlayer);
        }

        public void ArtifactReplayMiniPhaseStarted()
        {
            _chat.AddMessage($"started replaying artifact", _game.CurrentPlayer);
        }

        public void ArtifactReplayMiniPhaseEnded()
        {
            _chat.AddMessage($"stopped replaying artifact", _game.CurrentPlayer);
        }

        public void FulfillProphecyMiniPhaseStarted()
        {
            _chat.AddMessage($"is picking prophecy to fulfill", _game.CurrentPlayer);
        }

        public void FulfillProphecyMiniPhaseEnded()
        {
            _chat.AddMessage($"ended fullfilling prophecy", _game.CurrentPlayer);
        }

        public void LockCardMiniPhaseStarted()
        {
            _chat.AddMessage($"is picking mercenary to lock from buying for other users", _game.CurrentPlayer);
        }

        public void LockCardMiniPhaseEnded()
        {
            _chat.AddMessage($"stopped picking mercenary to lock from buying for other users", _game.CurrentPlayer);
        }

        public void BuffHeroMiniPhaseStarted()
        {
            _chat.AddMessage($"is picking hero to buff", _game.CurrentPlayer);
        }

        public void RotatePawnMiniPhaseEnded()
        {
            _chat.AddMessage($"stopped rotating pawn", _game.CurrentPlayer);
        }

        public void RotatePawnMiniPhaseStarted()
        {
            _chat.AddMessage($"started rotating pawn", _game.CurrentPlayer);
        }

        public void MercenaryRerollMiniPhaseStarted()
        {
            _chat.AddMessage($"is picking mercenary to reroll", _game.CurrentPlayer);
        }

        public void HeroCardRerollMiniPhaseStarted()
        {
            _chat.AddMessage($"is picking hero card to reroll", _game.CurrentPlayer);
        }

        public void HeroCardRerollMiniPhaseEnded()
        {
            _chat.AddMessage($"ended picking mercenary to reroll", _game.CurrentPlayer);
        }

        public void BuffHeroMiniPhaseEnded()
        {
            _chat.AddMessage($"stopped picking hero to buff", _game.CurrentPlayer);
        }

        public void BlockTileMiniPhaseStarted()
        {
            var stringToSend = $"started blocking tile";
            int chance = _random.Next(100);


            if (chance < 40)
            {
                stringToSend += " LIKE A MADMAN!";
            }
            _chat.AddMessage($"{stringToSend}", _game.CurrentPlayer);
        }

        public void BlockTileMiniPhaseEnded()
        {
            var stringToSend = $"stopped blocking tile";

            _chat.AddMessage($"{stringToSend}", _game.CurrentPlayer);
        }
    }
}