using BoardGameFrontend.Helpers;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Models;
using System;
using System.Windows.Controls;

namespace BoardGameFrontend.ChatLogManager
{
    public class TileChatLogManager
    {
        private ChatGameManager _chat { get; set; }
        private Game _game { get; set; }
        private Random _random = new Random();

        public TileChatLogManager(ChatGameManager chat, Game game)
        {
            _chat = chat;
            _game = game;
        }

        public void MoveOnTile(MoveOnTileData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"moved to tile {data.TileId}.", player);
            var rewardString = $"";
            rewardString += TileRewardDescriptionBuilder.BuildDescription(data.TileReward);
            _chat.AddMessage($"{rewardString}.", player);
        }

        public void TileReward(TileRewardData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;
            var rewardString = $"";
            rewardString += TileRewardDescriptionBuilder.BuildDescription(data.TileReward);
            _chat.AddMessage($"{rewardString}.", player);
        }


        public void MoveOnTileOnEvent(MoveOnTileOnEvent data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;
            var stringToChat = $"moved to tile by event on aura ";
            _chat.AddMessage($"{stringToChat}.", player);
        }

        public void GoldIntoMovementEvent(GoldIntoMovementEventData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            if(player == null) return;

            var stringToChat = $"converted gold into full movement ";
            _chat.AddMessage($"{stringToChat}.", player);
        }

        public void MovementConvertedEvent(FullMovementIntoEmptyEventData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            if(player == null) return;

            var stringToChat = $"converted full movement into two empty once.";
            _chat.AddMessage($"{stringToChat}.", player);
        }

        public void ReceivedResource(ResourceReceivedEventData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            if(player == null) return;
            
            _chat.AddMessage($"{data.ResourceInfo}.", player);
        }

        public void RotatePawn(RotateTileEventData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            if(player == null) return;
            
            _chat.AddMessage($"has rotate pawn to tile with Id {data.TileId}.", player);
        }

        public void SwappedTokens(SwapTokensDataEventData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            if(player == null) return;
            
            _chat.AddMessage($"has swapped position of tokens", player);
        }

        public void BlockedTileEvent(BlockedTileData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.PlayerId);
            if(player==null) return;

            var stringToSend = $"blocked tile with id {data.TileId}";
            int chance = _random.Next(100);


            if (chance < 40)
            {
                stringToSend += " ,Why? Cuz he is MADMAN!";
            }
            _chat.AddMessage($"{stringToSend}.", player);
        }

        



    }
}