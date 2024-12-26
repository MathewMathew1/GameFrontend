using BoardGameFrontend.Helpers;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Models;
using System.Linq;
using System.Windows.Controls;

namespace BoardGameFrontend.ChatLogManager
{
    public class ArtifactChatLogManager
    {
        private ChatGameManager _chat { get; set; }
        private Game _game { get; set; }

        public ArtifactChatLogManager(ChatGameManager chat, Game game)
        {
            _chat = chat;
            _game = game;
        }

        public void ArtifactPlayed(ArtifactPlayed data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"played {data.Artifact.NameEng}.", player);
            var rewardString = $"";
            rewardString += RewardDescriptionBuilder.BuildDescription(data.Reward);

            if(rewardString.Length > 0){
                _chat.AddMessage($"{rewardString}.", player);
            }
            
        }

        public void ArtifactRePlayed(ArtifactPlayed data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"replayed {data.Artifact.NameEng}.", player);
            var rewardString = $"";
            rewardString += RewardDescriptionBuilder.BuildDescription(data.Reward);
            _chat.AddMessage($"{rewardString}.", player);
        }

        public void ArtifactsToPickFrom(ArtifactToPickFromData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"received {data.Artifacts.Count} artifacts to pick from.", player);
        }

        public void ArtifactsGiven(ArtifactToPickFromData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"has received {data.Artifacts.Count} artifacts.", player);
        }

        public void ArtifactToPickFromDataForOtherUsers(ArtifactToPickFromDataForOtherUsers data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"has received {data.ArtifactsAmount} artifacts.", player);
        }

        public void ArtifactTakenData(ArtifactTakenData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"has taken {data.Artifacts.Count} artifacts.", player);
        }
        

        public void ArtifactsTakenOtherData(ArtifactTakenDataForOtherUsers data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"has taken {data.ArtifactsAmount} artifacts.", player);
        }

        public void ArtifactSummary(ArtifactToPickFromDataForOtherUsers data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"received {data.ArtifactsAmount} artifacts to pick from.", player);
        }

        public void ArtifactRerolled(ArtifactRerolledData data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"rerolled {data.Artifact.NameEng} artifact.", player);
        }

        public void ArtifactRerolledOtherData(ArtifactRerolledDataForOtherUsers data)
        {
            var player = _game.PlayerManager.GetPlayerById(data.Player.Id);
            if(player == null) return;

            _chat.AddMessage($"rerolled artifact.", player);
        }
    }
}