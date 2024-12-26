using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BoardGameFrontend.AutoMapper;
using BoardGameFrontend.Commands;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    public class RolayCardsManager : INotifyPropertyChanged
    {
        public ObservableCollection<RolayCardDisplay> RolayCards { get; private set; } = new ObservableCollection<RolayCardDisplay>();

        private RolayCardDisplay? _selectedRoyalCard;
        public RolayCardDisplay? SelectedRoyalCard
        {
            get { return _selectedRoyalCard; }
            set
            {
                if (_selectedRoyalCard != value)
                {
                    _selectedRoyalCard = value;
                    OnPropertyChanged(nameof(SelectedRoyalCard));
                }
            }
        }
        
        public RolayCardsManager(){
            SelectRoyalCardCommand = new RelayCommandWithTypes<int>(SelectRolayCardById);
        }

        public void SelectRolayCardById(int id)
        {
            if (SelectedRoyalCard != null)
            {
                SelectedRoyalCard.Selected = false;
            }


            var selectedRoyalCard = RolayCards.FirstOrDefault(card => card.Id == id);
            
            if (selectedRoyalCard != null && selectedRoyalCard.PickedByPlayer == null && selectedRoyalCard.BanishedCard == false)
            {
                SelectedRoyalCard = selectedRoyalCard;
                SelectedRoyalCard.Selected = true;
                return;
            }

            SelectedRoyalCard = null;
        }

        public void BanishRoyalCard(int id)
        {
            var card = RolayCards.FirstOrDefault(card => card.Id == id);
            if(card == null) return;

            if(SelectedRoyalCard?.Id == id){
                SelectedRoyalCard.Selected = false;
                SelectedRoyalCard = null;
            }

            card.BanishedCard = true;
        }


        public ICommand SelectRoyalCardCommand { get; private set; }

        public void SetRolayCards(List<RolayCard> rolayCards)
        {
            RolayCards.Clear();
            var sortedRolayCards = rolayCards.OrderBy(rolayCard => rolayCard.Faction.Id);
            foreach (var rolayCard in sortedRolayCards)
            {
                var rolayCardDisplay = AutoMapperConfig.Mapper.Map<RolayCardDisplay>(rolayCard);
                RolayCards.Add(rolayCardDisplay);
            }
            OnPropertyChanged(nameof(RolayCards));
        }

        public void SetRolayCardTaken(PlayerViewModel player, int cardId){
            var card = RolayCards.FirstOrDefault(x => x.Id == cardId);

            if(card == null) return;
         
            card.PickedByPlayer = player;

            if(SelectedRoyalCard?.Id == cardId){
                SelectedRoyalCard.Selected = false;
                SelectedRoyalCard = null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
