using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using BoardGameFrontend.Models;



namespace BoardGameFrontend.Managers
{
    public class ChatGameMessage{
        public required string Message {get; set;}
        public required string? PlayerName {get; set;}
        public required Color Color {get; set;}
    }

    public class ChatGameManager : INotifyPropertyChanged
    {
        public ObservableCollection<ChatGameMessage> ChatMessages { get; set; } = new ObservableCollection<ChatGameMessage>();
        public LobbyViewModelManager LobbyVM { get; set; }

        public ChatGameManager(LobbyViewModelManager lobbyVM){
            LobbyVM = lobbyVM;
        }

        public List<ChatGameMessage> ReversedChatMessages
        {
            get => ChatMessages.Reverse().ToList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddMessage(string message, PlayerInGameViewModel? player = null){
            if(player != null){
                ChatMessages.Add(new ChatGameMessage{Message=message, PlayerName=player.Name, Color = LobbyVM.GetColorForPlayer(player.Id)});
            }else{
                ChatMessages.Add(new ChatGameMessage{Message=message, PlayerName = null, Color=Colors.Blue});
            }
            
            OnPropertyChanged(nameof(ReversedChatMessages));
        }
    }
}