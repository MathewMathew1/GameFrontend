using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

using BoardGameFrontend.AutoMapper;
using BoardGameFrontend.Commands;
using BoardGameFrontend.Helpers;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    public class MercenaryManager : INotifyPropertyChanged
    {
        private int _amountOfMercenaries = 0;
        public int AmountOfMercenaries
        {
            get => _amountOfMercenaries;
            set
            {
                if (_amountOfMercenaries != value)
                {
                    _amountOfMercenaries = value;
                    OnPropertyChanged(nameof(AmountOfMercenaries));
                }
            }
        }
        private Game _game;

        private int _amountOfTossedMercenaries = 0;
        public int AmountOfTossedMercenaries
        {
            get => _amountOfTossedMercenaries;
            set
            {
                if (_amountOfTossedMercenaries != value)
                {
                    _amountOfTossedMercenaries = value;
                    OnPropertyChanged(nameof(AmountOfTossedMercenaries));
                }
            }
        }

        private MercenaryDisplay? _selectedMercenary;
        public MercenaryDisplay? SelectedMercenary
        {
            get { return _selectedMercenary; }
            set
            {
                if (_selectedMercenary != value)
                {
                    _selectedMercenary = value;
                    OnPropertyChanged(nameof(SelectedMercenary));
                }
            }
        }

        public ObservableCollection<MercenaryDisplay> BuyableMercenaries { get; private set; } = new ObservableCollection<MercenaryDisplay>();

        public MercenaryManager(Game game)
        {
            _game = game;
            SelectMercenaryCommand = new RelayCommandWithTypes<int>(SelectMercenaryById);
        }

        public ICommand SelectMercenaryCommand { get; private set; }

        public void SelectMercenaryById(int id)
        {
            if (SelectedMercenary != null)
            {
                SelectedMercenary.Selected = false;
            }


            var selectedMercenary = BuyableMercenaries.FirstOrDefault(card => card.InGameIndex == id);
            var miniPhaseWithSelectingMercenaryIsOn = _game.MiniPhaseManager.CurrentPhase?.Name == MiniPhaseType.MercenaryRerollPhase || _game.MiniPhaseManager.CurrentPhase?.Name == MiniPhaseType.LockMercenaryPhase;

            bool canSelectMercenary = selectedMercenary != null &&
                (selectedMercenary.CanBuy ||
                (miniPhaseWithSelectingMercenaryIsOn &&
                _game.IsUserControlledPlayersTurn));

            if (canSelectMercenary && selectedMercenary != null)
            {
                SelectedMercenary = selectedMercenary;
                SelectedMercenary.Selected = true;
                return;
            }

            SelectedMercenary = null;
        }

        public void SetData(MercenaryData mercenaryData)
        {
            AmountOfMercenaries = mercenaryData.RemainingMercenariesAmount;
            SetMercenaries(mercenaryData.BuyableMercenaries);
        }

        public void SetMercenaries(List<Mercenary> newBuyableMercenaries)
        {
            SelectedMercenary = null;
            BuyableMercenaries.Clear();
            foreach (var mercenary in newBuyableMercenaries)
            {
                var mercenaryDisplay = AutoMapperConfig.Mapper.Map<MercenaryDisplay>(mercenary);
                SetCanBuy(mercenaryDisplay);
                BuyableMercenaries.Add(mercenaryDisplay);
            }

            SetProphecyCompleted(_game.UserControlledPlayer);
            OnPropertyChanged(nameof(BuyableMercenaries));
        }

        public void SetProphecyCompleted(PlayerInGameViewModel player)
        {
            foreach (MercenaryDisplay mercenary in BuyableMercenaries)
            {
                if (mercenary.EffectId == null) continue;

                var requirement = ProphecyRequirementStore.GetRequirementById(mercenary.EffectId.Value);
                if (requirement == null) continue;

                mercenary.ProphecyRequirementsFulfilled = requirement.GetCompletedProphecy(player, mercenary);
            }
        }

        private void SetCanBuy(MercenaryDisplay mercenaryDisplay)
        {
            var isLockedByOtherPlayer = mercenaryDisplay.LockedByPlayerInfo != null && mercenaryDisplay.LockedByPlayerInfo.PlayerId != _game.UserControlledPlayer.Id;

            var resourceNeeded = mercenaryDisplay.ResourcesNeeded
                .Select(resource => new ResourceInfoData
                {
                    Name = resource.Name,
                    Amount = resource.Amount
                }).ToList();

            ReduceRequiredResourcesByAura(resourceNeeded, _game.UserControlledPlayer, mercenaryDisplay);

            resourceNeeded.ForEach(resource =>
            {
                if (resource.Name == ResourceType.Gold)
                {
                    resource.Amount -= mercenaryDisplay.GoldDecrease;
                }
            });

            var hasResourcesToBuy = false;
            if (!_game.UserControlledPlayer.PlayerAuraManager.DoesAuraExist(AurasType.BUY_CARDS_BY_ANY_RESOURCE))
            {
                hasResourcesToBuy = _game.UserControlledPlayer.ResourceManager.CheckForResources(resourceNeeded);
            }
            else
            {
                hasResourcesToBuy = _game.UserControlledPlayer.ResourceManager.CheckForResourceWithSubstitute(resourceNeeded);
            }

            var reqFullified = true;

            if (mercenaryDisplay.Req != null)
            {
                var req = RequirementMovementStore.GetRequirementById(mercenaryDisplay.Req.Value);
                if (req != null)
                {
                    reqFullified = req.CheckRequirements(_game.UserControlledPlayer);
                }

            }

            var canBuy = hasResourcesToBuy && reqFullified && !isLockedByOtherPlayer;
            mercenaryDisplay.CanBuy = canBuy;
        }

        public void SetCanBuyForAllMercenaries()
        {
            foreach (var mercenary in BuyableMercenaries)
            {
                SetCanBuy(mercenary);
            }
        }

        public void AddNewBuyableMercenary(Mercenary mercenary)
        {
            var mercenaryDisplay = AutoMapperConfig.Mapper.Map<MercenaryDisplay>(mercenary);
            SetCanBuy(mercenaryDisplay);
            SetProphecyCompleted(_game.UserControlledPlayer);
            BuyableMercenaries.Add(mercenaryDisplay);
        }

        public void RemoveMercenaryByGameIndex(int gameIndex)
        {
            var mercenary = BuyableMercenaries.FirstOrDefault(mercenary => mercenary.InGameIndex == gameIndex);

            if (mercenary == null) return;

            if (mercenary.InGameIndex == SelectedMercenary?.InGameIndex)
            {
                SelectedMercenary = null;
            }

            BuyableMercenaries.Remove(mercenary);
            OnPropertyChanged(nameof(BuyableMercenaries));
        }

        public void ChangeMercenariesAmountData(MercenariesLeftData mercenariesLeftData)
        {
            AmountOfTossedMercenaries = mercenariesLeftData.TossedMercenariesAmount;
            AmountOfMercenaries = mercenariesLeftData.MercenariesAmount;
        }

        public void EndOfRound(EndOfRoundMercenaryData endOfRoundMercenaryData)
        {
            ChangeMercenariesAmountData(endOfRoundMercenaryData.MercenariesLeftData);
            SetMercenaries(endOfRoundMercenaryData.Mercenaries);
        }

        public void PreMercenaryRerolled(PreMercenaryRerolled mercenaryRerolledData){
            if(mercenaryRerolledData.MercenaryReplacement != null){
                AddNewBuyableMercenary(mercenaryRerolledData.MercenaryReplacement);
            }

            ChangeMercenariesAmountData(mercenaryRerolledData.MercenariesLeftData);
        }

        public void MercenaryRerolled(MercenaryRerolledData mercenaryRerolledData)
        {
            RemoveMercenaryByGameIndex(mercenaryRerolledData.Card.InGameIndex);
            
            
            ChangeMercenariesAmountData(mercenaryRerolledData.MercenariesLeftData);

            _game.MiniPhaseManager.SetCurrentPhase(null);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LockMercenary(LockMercenaryData lockMercenaryData)
        {
            var mercenary = BuyableMercenaries.First(m => m.InGameIndex == lockMercenaryData.MercenaryId);

            if (mercenary == null) return;

            mercenary.LockedByPlayerInfo = lockMercenaryData.LockMercenary;

            if (mercenary.InGameIndex == SelectedMercenary?.InGameIndex)
            {
                SelectedMercenary = null;
            }
            SetCanBuyForAllMercenaries();
        }

        public void ReduceRequiredResourcesByAura(List<ResourceInfoData> resources, PlayerInGameViewModel player, MercenaryDisplay mercenary)
        {
            var auraDiscount = 0;
            foreach (var a in player.PlayerAuraManager.AurasTypes)
            {
                if (a.Aura == AurasType.CHEAPER_BUILDINGS && mercenary.TypeCard == MercenaryHelper.BuildingCardType)
                {
                    resources.ForEach(r =>
                    {
                        if (r.Name == ResourceType.Gold && r.Amount > 0)
                        {
                            auraDiscount += 2; 
                            r.Amount = Math.Max(r.Amount - 2, 0);
                        }
                    });
                }
                if (a.Aura == AurasType.MAKE_CHEAPER_MERCENARIES && a.Value1 == mercenary.Faction?.Id)
                {
                    foreach (var r in resources)
                    {
                        if (r.Name == ResourceType.Gold && r.Amount > 0)
                        {
                            auraDiscount += 1; 
                            r.Amount -= 1;
                        }
                    }
                }
            }

            mercenary.AuraDiscount = auraDiscount;
        }
    }
}
