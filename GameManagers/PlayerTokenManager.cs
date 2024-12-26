using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    public  class PlayerTokenManager: INotifyPropertyChanged
    {
        public ObservableCollection<Token> Tokens{ get; set; } = new ObservableCollection<Token>();

        public void AddToken(Token token){
            if(token.InStartingPool == false) return;
            Tokens.Add(token);
        }

        public void AddTokens(List<Token> tokens){
            tokens.ForEach(token => {
                Tokens.Add(token);
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    
    }
}