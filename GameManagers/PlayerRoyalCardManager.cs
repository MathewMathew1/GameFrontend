using System.Collections.ObjectModel;
using System.ComponentModel;
using BoardGameFrontend.AutoMapper;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    public  class PlayerRolayCardManager: INotifyPropertyChanged
    {
        public ObservableCollection<RolayCardDisplay> RolayCards { get; set; } = new ObservableCollection<RolayCardDisplay>();
        private int _signetsNeededForNextCard = 3;
        public int SignetsNeededForNextCard
        {
            get => _signetsNeededForNextCard;
            set
            {
                if (_signetsNeededForNextCard != value)
                {
                    _signetsNeededForNextCard = value;
                    OnPropertyChanged(nameof(SignetsNeededForNextCard));
                }
            }
        }


        public void SetSignetsNeededForNextCard(int signetsNeededForNextCard){
            SignetsNeededForNextCard = signetsNeededForNextCard;
        }

        public void AddRolayCard(RolayCard card){
            var rolayCardDisplay = AutoMapperConfig.Mapper.Map<RolayCardDisplay>(card);
            RolayCards.Add(rolayCardDisplay);
        }

        public void SetRoyalData(RoyalCardsPlayerData royalCardsPlayerData){
            SignetsNeededForNextCard = royalCardsPlayerData.SignetsNeededForNextCard;
            royalCardsPlayerData.RoyalCards.ForEach(card => {
                var rolayCardDisplay = AutoMapperConfig.Mapper.Map<RolayCardDisplay>(card);
                RolayCards.Add(rolayCardDisplay);
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    
    }
}