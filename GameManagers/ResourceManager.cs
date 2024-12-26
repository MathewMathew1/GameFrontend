using System.Collections.Generic;
using System.ComponentModel;
using BoardGameFrontend.Models;
using System.Linq;
using System;

namespace BoardGameFrontend.Managers
{
    public class ResourceManager : INotifyPropertyChanged
    {
        public List<ResourceType> allowedSubstituteResources = new List<ResourceType>
        {
            ResourceType.Wood,
            ResourceType.Iron,
            ResourceType.Gems,
            ResourceType.Niter
        };

        private int _goldIncome;
        public int GoldIncome
        {
            get => _goldIncome;
            set
            {
                if (_goldIncome != value)
                {
                    _goldIncome = value;
                    OnPropertyChanged(nameof(GoldIncome));
                }
            }
        }

        private Dictionary<ResourceType, ResourceInfo> _resources;
        public event PropertyChangedEventHandler? PropertyChanged;

        public int GoldAmount
        {
            get => _resources.ContainsKey(ResourceType.Gold) ? _resources[ResourceType.Gold].Amount : 0;
            set
            {
                if (_resources.ContainsKey(ResourceType.Gold))
                {
                    if (_resources[ResourceType.Gold].Amount != value)
                    {
                        _resources[ResourceType.Gold].Amount = value;
                        OnPropertyChanged(nameof(GoldAmount)); // Notify property change
                        OnPropertyChanged(nameof(Resources)); // Notify overall resources change
                    }
                }
            }
        }

        public Dictionary<ResourceType, ResourceInfo> Resources
        {
            get => _resources;
            private set
            {
                if (_resources != value)
                {
                    _resources = value;
                    OnPropertyChanged(nameof(Resources));
                    OnPropertyChanged(nameof(GoldAmount));
                }
            }
        }

        public ResourceManager(Dictionary<ResourceType, int> resources)
        {
            _resources = InitializeResources(resources);
        }

        public void SetupResources(Dictionary<ResourceType, int> resources)
        {
            foreach (var resourceType in resources.Keys)
            {
          
                    resources.TryGetValue(resourceType, out int resourceAmount);

                    _resources[resourceType].Amount = resourceAmount;
                

            }
            UpdateBuyableThings?.Invoke();
        }

        public static Dictionary<ResourceType, ResourceInfo> InitializeResources(Dictionary<ResourceType, int> resources)
        {
            var _resources = new Dictionary<ResourceType, ResourceInfo>();

            var resourceNameMap = new Dictionary<ResourceType, string>
        {
            { ResourceType.Gold, "Gold" },
            { ResourceType.Wood, "Wood" },
            { ResourceType.Iron, "Iron" },
            { ResourceType.Gems, "Gems" },
            { ResourceType.Niter, "Niter" },
            { ResourceType.MysticFog, "Mystic fog" }
        };

            foreach (var resourceType in resourceNameMap.Keys)
            {
                string nameInJson = resourceNameMap[resourceType];

                var matchingResourceJson = ResourcesFromJson.ResourceFromJsonList
                    .FirstOrDefault(r => r.NameEng == nameInJson);

                if (matchingResourceJson != null)
                {
                    resources.TryGetValue(resourceType, out int resourceAmount);

                    _resources[resourceType] = new ResourceInfo(resourceAmount, matchingResourceJson);
                }

            }

            return _resources;
        }

        public void EndOfTurnIncome()
        {
            foreach (var resource in _resources)
            {
                if (resource.Key != ResourceType.Gold)
                {
                    _resources[resource.Key].Amount = 0;
                }
            }
            UpdateBuyableThings?.Invoke();
        }

        public void EndOfRoundIncome()
        {
            foreach (var resource in _resources)
            {
                if (resource.Key == ResourceType.Gold)
                {
                    _resources[resource.Key].Amount += GoldIncome;
                }
                else
                {
                    _resources[resource.Key].Amount = 0;
                }
            }
            UpdateBuyableThings?.Invoke();
        }

        public Action? UpdateBuyableThings { get; set; }

        public void AddGoldIncome(int addedGoldIncome)
        {
            GoldIncome += addedGoldIncome;
        }

        public int GetResourceAmount(ResourceType resourceType)
        {
            return Resources.ContainsKey(resourceType) ? Resources[resourceType].Amount : 0;
        }

        public void AddResources(List<Resource> resourcesToAdd)
        {
            foreach (var resource in resourcesToAdd)
            {
                AddResource(resource.Type, resource.Amount);
            }
            UpdateBuyableThings?.Invoke();
        }

        public void AddResource(ResourceType resourceType, int amount)
        {
            if (Resources.ContainsKey(resourceType))
            {
                Resources[resourceType].Amount += amount;
                UpdateBuyableThings?.Invoke();
            }
        }

        public bool SubtractResource(ResourceType resourceType, int amount)
        {
            if (Resources.ContainsKey(resourceType) && Resources[resourceType].Amount >= amount)
            {
                Resources[resourceType].Amount -= amount;
                UpdateBuyableThings?.Invoke();
                return true;
            }
            return false;
        }

        public Dictionary<ResourceType, ResourceInfo> GetResources()
        {
            return Resources;
        }

        public bool CheckForResources(List<ResourceInfoData> resources)
        {
            foreach (var resource in resources)
            {
                int availableAmount = GetResourceAmount(resource.Name);

                if (availableAmount < resource.Amount)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckForResourceWithSubstitute(List<ResourceInfoData> resources)
        {
            List<ResourceType> resourcesWithAmountGreaterThanZero = _resources
            .Where(resource => allowedSubstituteResources.Contains(resource.Key) && resource.Value.Amount > 0)
            .Select(resource => resource.Key)
            .ToList();

            var substituteResourcesUsed = 0;
            foreach (var resource in resources)
            {
                if (allowedSubstituteResources.Contains(resource.Name))
                {
                    substituteResourcesUsed += 1;
                    if (substituteResourcesUsed > resourcesWithAmountGreaterThanZero.Count)
                    {
                        return false;
                    }
                    else
                    {
                        continue;
                    }
                }

                int availableAmount = GetResourceAmount(resource.Name);

                if (availableAmount < resource.Amount)
                {
                    return false;
                }
            }


            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}