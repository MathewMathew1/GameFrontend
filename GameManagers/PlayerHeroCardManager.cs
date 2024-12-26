using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BoardGameFrontend.AutoMapper;
using BoardGameFrontend.Commands;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    public class PlayerHeroCardManager : INotifyPropertyChanged
    {
        public ObservableCollection<HeroCardDisplay> HeroCardsLeft { get; set; } = new ObservableCollection<HeroCardDisplay>();
        public ObservableCollection<HeroCardDisplay> HeroCardsRight { get; set; } = new ObservableCollection<HeroCardDisplay>();

        private CurrentHeroCard? _currentHeroCard;
        public CurrentHeroCard? CurrentHeroCard
        {
            get => _currentHeroCard;
            set
            {
                if (_currentHeroCard != value)
                {
                    _currentHeroCard = value;
                    OnPropertyChanged(nameof(CurrentHeroCard));
                    UpdateBuyableThings?.Invoke();
                }
            }
        }

        private BorderVisual? _currentBorder;
        public BorderVisual? CurrentBorder
        {
            get { return _currentBorder; }
            set
            {
                if (_currentBorder != value)
                {
                    _currentBorder = value;
                    OnPropertyChanged(nameof(CurrentBorder));
                }
            }
        }

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

        public PlayerHeroCardManager()
        {
            SelectHeroCardCommand = new RelayCommandWithTypes<int>(SelectHeroCardById);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddHeroCardLeft(HeroCardDisplay heroCard)
        {
            HeroCardsLeft.Add(heroCard);
            UpdateBuyableThings?.Invoke();
        }

        public void SetAllDate(PlayerHeroData playerHeroData)
        {
            foreach (var heroCard in playerHeroData.LeftHeroCards)
            {
                var heroCardDisplay = AutoMapperConfig.Mapper.Map<HeroCardDisplay>(heroCard);
                HeroCardsLeft.Add(heroCardDisplay);
            }

            foreach (var heroCard in playerHeroData.RightHeroCards)
            {
                var heroCardDisplay = AutoMapperConfig.Mapper.Map<HeroCardDisplay>(heroCard);
                HeroCardsRight.Add(heroCardDisplay);
            }

            if (playerHeroData.CurrentHeroCard != null)
            {
                CurrentBorder = new BorderVisual(playerHeroData.CurrentHeroCard.HeroCard.Faction.Id);
                var currentHeroCard = AutoMapperConfig.Mapper.Map<CurrentHeroCard>(playerHeroData.CurrentHeroCard);
                CurrentHeroCard = currentHeroCard;
            }
        }

        public void ResetCurrentHeroCard()
        {
            if (CurrentHeroCard == null) return;

            if (CurrentHeroCard.LeftSide)
            {
                HeroCardsLeft.Add(CurrentHeroCard.HeroCard);
            }
            else
            {
                HeroCardsRight.Add(CurrentHeroCard.HeroCard);
            }

            if (CurrentHeroCard.ReplacedHeroCard != null)
            {
                if (CurrentHeroCard.ReplacedHeroCard.WasOnLeftSide)
                {
                    HeroCardsLeft.Add(CurrentHeroCard.ReplacedHeroCard.HeroCard);
                }
                else
                {
                    HeroCardsRight.Add(CurrentHeroCard.ReplacedHeroCard.HeroCard);
                }
            }

            CurrentBorder = null;
            CurrentHeroCard = null;
            UpdateBuyableThings?.Invoke();
        }

        public void SetCurrentHeroCard(CurrentHeroCard currentHeroCardData){
            CurrentHeroCard = currentHeroCardData;
            CurrentBorder = new BorderVisual(currentHeroCardData.HeroCard.Faction.Id);
        }

        public void ResetCurrentHeroCardReverse()
        {
            if (CurrentHeroCard == null) return;

            if (CurrentHeroCard.LeftSide)
            {
                HeroCardsRight.Add(CurrentHeroCard.UnUsedHeroCard);
            }
            else
            {
                HeroCardsLeft.Add(CurrentHeroCard.UnUsedHeroCard);
            }

            if (CurrentHeroCard.ReplacedHeroCard != null)
            {
                if (CurrentHeroCard.ReplacedHeroCard.WasOnLeftSide)
                {
                    HeroCardsLeft.Add(CurrentHeroCard.ReplacedHeroCard.HeroCard);
                }
                else
                {
                    HeroCardsRight.Add(CurrentHeroCard.ReplacedHeroCard.HeroCard);
                }
            }

            CurrentBorder = null;
            CurrentHeroCard = null;
            UpdateBuyableThings?.Invoke();
        }

        public void RemoveHeroCardById(int heroCardId)
        {
            var itemsToRemoveLeft = HeroCardsLeft.Where(x => x.Id == heroCardId).ToList();
            var itemsToRemoveRight = HeroCardsRight.Where(x => x.Id == heroCardId).ToList();

            foreach (var item in itemsToRemoveLeft)
            {
                HeroCardsLeft.Remove(item);
            }

            foreach (var item in itemsToRemoveRight)
            {
                HeroCardsRight.Remove(item);
            }
        }

        public Action? UpdateBuyableThings { get; set; }

        public void AddHeroCardRight(HeroCardDisplay heroCard)
        {
            HeroCardsRight.Add(heroCard);
            UpdateBuyableThings?.Invoke();
        }

        public int AmountOfHeroesOfFaction(int factionId)
        {
            var amountOfHerosOfFractionOnTheLeft = HeroCardsLeft.Count(card => card.Faction.Id == factionId);
            var amountOfHerosOfFractionOnTheRight = HeroCardsRight.Count(card => card.Faction.Id == factionId);
            var amountOfHerosOfFractionOnTheCenter = CurrentHeroCard?.HeroCard.Faction.Id == factionId ? 1 : 0;

            var amountHeroesOfThatFactions = amountOfHerosOfFractionOnTheCenter + amountOfHerosOfFractionOnTheLeft + amountOfHerosOfFractionOnTheRight;

            return amountHeroesOfThatFactions;
        }

        public void SelectHeroCardById(int id)
        {
            if (SelectedHeroCard != null)
            {
                SelectedHeroCard.Selected = false;
            }


            var SelectedHeroCardLeft = HeroCardsLeft.FirstOrDefault(card => card.Id == id);
            if (SelectedHeroCardLeft != null)
            {
                SelectedHeroCard = SelectedHeroCardLeft;
                SelectedHeroCard.Selected = true;
                return;
            }

            var SelectedHeroCardRight = HeroCardsRight.FirstOrDefault(card => card.Id == id);
            if (SelectedHeroCardRight != null)
            {
                SelectedHeroCard = SelectedHeroCardRight;
                SelectedHeroCard.Selected = true;
                return;
            }

            SelectedHeroCard = null;
        }

        public HeroCardDisplay? GetHeroCardById(int id)
        {
            var HeroCard = HeroCardsLeft.FirstOrDefault(card => card.Id == id);
            if (HeroCard != null)
            {
                return HeroCard;
            }

            HeroCard = HeroCardsRight.FirstOrDefault(card => card.Id == id);
            if (HeroCard != null)
            {
                return HeroCard;
            }

            return null;
        }



    }


}