using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    public class ChatLobbyMessage
    {
        public required string Message { get; set; }
        public required string PlayerName { get; set; }
        public required Color Color {get; set;}
    }

    public class ChatLobbyManager : INotifyPropertyChanged
    {
        public ObservableCollection<ChatLobbyMessage> ChatMessages { get; set; } = new ObservableCollection<ChatLobbyMessage>();

        public LobbyViewModelManager LobbyVM { get; set; }

        public ChatLobbyManager(LobbyViewModelManager lobbyVM){
            LobbyVM = lobbyVM;
        }

        public List<ChatLobbyMessage> ReversedChatMessages
        {
            get => ChatMessages.Reverse().ToList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddMessage(string message, PlayerViewModel? player = null)
        {
            if(player != null){
                ChatMessages.Add(new ChatLobbyMessage{Message=message, PlayerName=player.Name, Color = LobbyVM.GetColorForPlayer(player.Id)});
            }else{
                ChatMessages.Add(new ChatLobbyMessage{Message=message, PlayerName = null, Color=Colors.Blue});
            }
            
            OnPropertyChanged(nameof(ReversedChatMessages));
        }
    }
}