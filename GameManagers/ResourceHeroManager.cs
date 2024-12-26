using System.Collections.Generic;
using System.ComponentModel;
using BoardGameFrontend.Models;
using System.Linq;
using System;
using BoardGameFrontend.Helpers;

namespace BoardGameFrontend.Managers
{
    public class ResourceHeroManager: INotifyPropertyChanged
    {
        public ObservableDictionary<ResourceHeroType, ResourceHeroInfo> Resources { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public Action? UpdateBuyableThings { get; set; }

        public ResourceHeroManager()
        {
            Resources = InitializeResources();
            UpdateBuyableThings?.Invoke();
        }

        public void SetupResources(Dictionary<ResourceHeroType, int> resources){
            foreach (var resourceType in resources.Keys)
            {
                Resources[resourceType].Amount = resources[resourceType];
            }
            UpdateBuyableThings?.Invoke();
        }

        public static ObservableDictionary<ResourceHeroType, ResourceHeroInfo> InitializeResources()
        {
            var _resources = new ObservableDictionary<ResourceHeroType, ResourceHeroInfo>();

            var resourceNameMap = new Dictionary<ResourceHeroType, string>
            {
                { ResourceHeroType.Siege, "Siege" },
                { ResourceHeroType.Magic, "Magic" },
                { ResourceHeroType.Army, "Army" },
                { ResourceHeroType.Signet, "Signet" },
            };

            foreach (var resourceType in resourceNameMap.Keys)
            {
                string nameInJson = resourceNameMap[resourceType];

                var matchingResourceJson = ResourcesHeroFromJson.ResourceFromJsonList
                    .FirstOrDefault(r => r.Name == nameInJson);

                if (matchingResourceJson != null)
                {

                    _resources[resourceType] = new ResourceHeroInfo(0, matchingResourceJson);
                }

            }

            return _resources;
        }

        public int GetResourceAmount(ResourceHeroType resourceType)
        {
            return Resources[resourceType].Amount;
        }

        public void AddResource(ResourceHeroType resourceType, int amount)
        {

            Resources[resourceType].Amount += amount;
            UpdateBuyableThings?.Invoke();
        }

        public bool SubtractResource(ResourceHeroType resourceType, int amount)
        {
            if (amount < 0){

            }
                

            if (Resources[resourceType].Amount < amount)
                return false;

            Resources[resourceType].Amount -= amount;
            UpdateBuyableThings?.Invoke();
            return true;           
        }

        public Dictionary<ResourceHeroType, ResourceHeroInfo> GetResources(){
            return Resources;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}