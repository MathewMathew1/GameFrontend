
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Managers
{
    public class PlayerAuraManager : INotifyPropertyChanged
    {
        private ObservableCollection<AuraTypeWithLongevity> _aurasTypes = new ObservableCollection<AuraTypeWithLongevity>();
        public ObservableCollection<AuraTypeWithLongevity> AurasTypes
        {
            get => _aurasTypes;
            set
            {
                if (_aurasTypes != value)
                {
                    if (_aurasTypes != null)
                        _aurasTypes.CollectionChanged -= OnVisitedPlacesChanged;

                    _aurasTypes = value;
                    OnPropertyChanged(nameof(AurasTypes));

                    if (_aurasTypes != null)
                        _aurasTypes.CollectionChanged += OnVisitedPlacesChanged;
                }
            }
        }

        public PlayerAuraManager()
        {
            _aurasTypes.CollectionChanged += OnVisitedPlacesChanged;
        }

        private void OnVisitedPlacesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AurasTypes));
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void AddAura(AuraTypeWithLongevity aura)
        {
            AurasTypes.Add(aura);
            UpdateBuyableThings?.Invoke();
        }

        public void SetupAuras(List<AuraTypeWithLongevity> auras)
        {
            AurasTypes.Clear();
            auras.ForEach(aura => {
                AurasTypes.Add(aura);
            });
            UpdateBuyableThings?.Invoke();
        }

        public void RemoveAuraAtIndex(int index)
        {
            AurasTypes.RemoveAt(index);
            UpdateBuyableThings?.Invoke();
        }

        public bool DoesAuraExist(AurasType auraType)
        {
            return AurasTypes.Any(aura => aura.Aura == auraType);
        }

        public int GetAuraIndex(AurasType auraType)
        {
            return AurasTypes.ToList().FindIndex(aura => aura.Aura == auraType);
        }

        public void RemoveAuraOfType(AurasType aurasType)
        {
            var index = AurasTypes.ToList().FindIndex(aura => aura.Aura == aurasType);

            if (index == -1) return;

            AurasTypes.RemoveAt(index);
            UpdateBuyableThings?.Invoke();
        }

        public void ResetAura()
        {
            var permanentAuras = AurasTypes.Where(aura => aura.Permanent).ToList();

            AurasTypes.Clear();

            foreach (var aura in permanentAuras)
            {
                AurasTypes.Add(aura);
            }

            UpdateBuyableThings?.Invoke();
        }




        public int RemoveAurasOfTypeAndReturnAmountCount(AurasType auraType)
        {
            var amountRemoved = 0;

            for (var i = AurasTypes.Count - 1; i >= 0; i--)
            {
                if (auraType == AurasType.RETURN_TO_CENTER_ON_MOVEMENT)
                {
                    amountRemoved += 1;
                    AurasTypes.RemoveAt(i);
                }
            }
            UpdateBuyableThings?.Invoke();
            return amountRemoved;
        }

        public Action? UpdateBuyableThings { get; set; }
    }


}