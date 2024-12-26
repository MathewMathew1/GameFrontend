using BoardGameFrontend.Models;
using BoardGameFrontend.Managers;

namespace BoardGameFrontend.ChatLogManager
{
    public class LobbyChatLogManager
    {
        private ChatLobbyManager _chat {get; set;}

        public LobbyChatLogManager(ChatLobbyManager chat)
        {
            _chat = chat;
        }

        public void PlayerJoined(PlayerViewModel player){
            _chat.AddMessage("joined the lobby.", player);
        }

        public void PlayerDisconnected(PlayerViewModel player){
            _chat.AddMessage("disconnected from the lobby.", player);
        }

        public void PlayerConnected(PlayerViewModel player){
            _chat.AddMessage("rejoined the lobby.", player);
        }

        public void PlayerLeft(PlayerViewModel player){
            _chat.AddMessage("left the lobby." , player);
        }

        public void DestroyedLobby(PlayerViewModel player){
            _chat.AddMessage("destroyed the lobby.", player);
        }

        public void MessageSend(PlayerViewModel user, string message){
            _chat.AddMessage($"{message}", user);
        }

        public void GameStarted(){
            _chat.AddMessage("Game started");
        }

    }
}