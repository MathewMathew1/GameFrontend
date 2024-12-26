using System;
using System.ComponentModel;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{


    public class UserDataManager : INotifyPropertyChanged
    {
        private string _token;
        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                OnPropertyChanged(nameof(Token));
            }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private Guid _id;
        public Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public UserDataManager(UserData data)
        {
            _token = data.Token;
            _username = data.Username;
            _id = data.Id;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update(UserData newData)
        {
            Token = newData.Token;
            Username = newData.Username;
            Id = newData.Id;
        }
    }
}