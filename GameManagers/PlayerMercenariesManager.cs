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
    public class PlayerMercenariesManager : INotifyPropertyChanged
    {
        public ObservableCollection<MercenaryDisplay> Mercenaries { get; set; } = new ObservableCollection<MercenaryDisplay>();

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

        public PlayerMercenariesManager()
        {
            SelectMercenaryCommand = new RelayCommandWithTypes<int>(SelectMercenaryById);
        }

        public ICommand SelectMercenaryCommand { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddMercenary(MercenaryDisplay mercenary){
            Mercenaries.Add(mercenary);
        }

        public MercenaryDisplay? GetMercenaryById(int id){
            return Mercenaries.FirstOrDefault(m=>m.InGameIndex==id);
        }

        public void SetProphecyCompleted(PlayerInGameViewModel player){
            foreach (MercenaryDisplay mercenary in Mercenaries){
                if(mercenary.EffectId == null) continue;

                var requirement = ProphecyRequirementStore.GetRequirementById(mercenary.EffectId.Value);
                if(requirement == null) continue;
                
                mercenary.ProphecyRequirementsFulfilled = requirement.GetCompletedProphecy(player, mercenary);
            }
        }

        public void SetAlwaysCompleteProphecy(int mercenaryId){
            var mercenary = Mercenaries.First(_mercenary => _mercenary.InGameIndex == mercenaryId);
            if(mercenary == null) return;

            mercenary.IsAlwaysFulfilled = true;
        }

        public void SetAllMercenaries(List<Mercenary> mercenaries, PlayerInGameViewModel player){
            foreach (var mercenary in mercenaries)
            {
                var mercenaryDisplay = AutoMapperConfig.Mapper.Map<MercenaryDisplay>(mercenary);
                Mercenaries.Add(mercenaryDisplay);
            }
            SetProphecyCompleted(player);
        }

        public void SelectMercenaryById(int id)
        {
            if (SelectedMercenary != null)
            {
                SelectedMercenary.Selected = false;
            }

            var selectedMercenary = Mercenaries.FirstOrDefault(card => card.InGameIndex == id);

            if (selectedMercenary != null)
            {
                SelectedMercenary = selectedMercenary;
                SelectedMercenary.Selected = true;
                return;
            }

            SelectedMercenary = null;
        }

        
    }

    
}