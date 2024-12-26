using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BoardGameFrontend.Models;
using BoardGameFrontend.Commands;
using BoardGameFrontend.AutoMapper;

namespace BoardGameFrontend.Managers
{
    public class HeroCardsBoardManager : INotifyPropertyChanged
    {
        public ObservableCollection<HeroCardCombinedDisplay> HeroCards { get; set; } = new ObservableCollection<HeroCardCombinedDisplay>();

        private HeroCardDisplay? _selectedHeroCard;
        public HeroCardDisplay? SelectedHeroCard
        {
            get { return _selectedHeroCard; }
            set
            {
                if (_selectedHeroCard != value)
                {
                    _selectedHeroCard = value;
                    OnPropertyChanged(nameof(SelectedHeroCard));
                }
            }
        }

        public ICommand SelectHeroCardCommand { get; private set; }

        public HeroCardsBoardManager()
        {
            SelectHeroCardCommand = new RelayCommandWithTypes<int>(SelectHeroCardById);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetHeroCards(List<HeroCardCombined> newHeroCards)
        {
            HeroCards.Clear();
            foreach (var heroCardCombined in newHeroCards)
            {
                var heroCard = AutoMapperConfig.Mapper.Map<HeroCardCombinedDisplay>(heroCardCombined);

                HeroCards.Add(heroCard);
            }
            OnPropertyChanged(nameof(HeroCards));
        }

        public void SelectHeroCardById(int id)
        {
            if(SelectedHeroCard !=null){
                SelectedHeroCard.Selected = false;
            }
            

            var SelectedHeroCardLeft = HeroCards.FirstOrDefault(card => card.LeftSide.Id == id && card.PlayerWhoPickedCard == null);
            if(SelectedHeroCardLeft != null){
                SelectedHeroCard = SelectedHeroCardLeft .LeftSide;
                SelectedHeroCard.Selected = true;
                return;
            }

            var SelectedHeroCardRight = HeroCards.FirstOrDefault(card => card.RightSide.Id == id && card.PlayerWhoPickedCard == null);
            if(SelectedHeroCardRight != null){
                SelectedHeroCard = SelectedHeroCardRight.RightSide;
                SelectedHeroCard.Selected = true;
                return;
            }

            SelectedHeroCard = null;
        }

        public HeroCardCombinedDisplay? SetCardAsPickedByUser(int id, PlayerViewModel player){
            var pickedCard = HeroCards.FirstOrDefault(card => card.LeftSide.Id == id || card.RightSide.Id == id);
            if(pickedCard != null){
                pickedCard.PlayerWhoPickedCard = player;
                if(id == SelectedHeroCard?.Id){
                    SelectedHeroCard.Selected = false;
                    SelectedHeroCard = null;
                }
            }   

            return pickedCard;  
        }

        
    }

    
}