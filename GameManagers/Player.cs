using System.ComponentModel;
using BoardGameFrontend.Managers;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System;
using BoardGameFrontend.AutoMapper;
using System.Collections.Generic;
using System.Linq;
using BoardGameFrontend.Helpers;
using System.Collections.Specialized;

namespace BoardGameFrontend.Models
{
    public class PlayerViewModelData
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required ResourceManagerViewModel ResourceManager { get; set; }
    }

    public class Player
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
    }

    public class PlayerViewModel : INotifyPropertyChanged
    {

        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;



        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PlayerInGameViewModel : INotifyPropertyChanged
    {
        private bool _isCurrentPlayer;
        public bool IsCurrentPlayer
        {
            get => _isCurrentPlayer;
            set
            {
                if (_isCurrentPlayer != value)
                {
                    _isCurrentPlayer = value;
                    OnPropertyChanged(nameof(IsCurrentPlayer));
                }
            }
        }

        private bool _isConnected = true;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }
        public required ResourceManager ResourceManager { get; set; }
        public required ResourceHeroManager ResourceHeroManager { get; set; } = new ResourceHeroManager { };
        public PlayerHeroCardManager PlayerHeroCardManager { get; set; } = new PlayerHeroCardManager { };
        public PlayerMercenariesManager PlayerMercenariesManager { get; set; } = new PlayerMercenariesManager();
        public ObservableCollection<ArtifactDisplay> Artifacts { get; set; } = new ObservableCollection<ArtifactDisplay>();
        public PlayerRolayCardManager PlayerRoyalCardManager { get; set; } = new PlayerRolayCardManager();
        public List<EndGameAura> EndGameAuras { get; set; } = new List<EndGameAura>();
        public PlayerTokenManager TokenManager { get; set; } = new PlayerTokenManager();
        public PlayerAuraManager PlayerAuraManager { get; set; } = new PlayerAuraManager();
        public PlayerTimeManager TimerManager { get; set; } = new PlayerTimeManager();
        public PlayerArtifactManager PlayerArtifactManager { get; set; }
        private ObservableCollection<string> _boolAdditionalStorage = new ObservableCollection<string>();
        public ObservableCollection<string> BoolAdditionalStorage
        {
            get => _boolAdditionalStorage;
            set
            {
                if (_boolAdditionalStorage != value)
                {
                    if (_boolAdditionalStorage != null)
                        _boolAdditionalStorage.CollectionChanged -= OnVisitedPlacesChanged;

                    _boolAdditionalStorage = value;
                    OnPropertyChanged(nameof(BoolAdditionalStorage));

                    if (_boolAdditionalStorage != null)
                        _boolAdditionalStorage.CollectionChanged += OnVisitedPlacesChanged;
                }
            }
        }



        private void OnVisitedPlacesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(BoolAdditionalStorage));
        }

        public int ArtifactsAmountOtherUsers { get; set; } = 0;

        private int _morale;
        public int Morale
        {
            get => _morale;
            set
            {
                if (_morale != value)
                {
                    _morale = value;
                    OnPropertyChanged(nameof(Morale));
                }
            }
        }

        private int _points;
        public int Points
        {
            get => _points;
            set
            {
                if (_points != value)
                {
                    _points = value;
                    OnPropertyChanged(nameof(Points));
                }
            }
        }

        private bool _alreadyPlayedCurrentPhase = false;
        public bool AlreadyPlayedCurrentPhase
        {
            get => _alreadyPlayedCurrentPhase;
            set
            {
                if (_alreadyPlayedCurrentPhase != value)
                {
                    _alreadyPlayedCurrentPhase = value;
                    OnPropertyChanged(nameof(AlreadyPlayedCurrentPhase));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public PlayerInGameViewModel()
        {
            PlayerArtifactManager = new PlayerArtifactManager(this);
            PlayerRoyalCardManager.PropertyChanged += (sender, args) =>
            {
                OnPropertyChanged(nameof(ProgressText));
                OnPropertyChanged(nameof(ProgressPercentage));
            };
            foreach (var resource in ResourceHeroManager.Resources)
            {
                resource.Value.PropertyChanged += (sender, args) =>
                {
                    if (resource.Key == ResourceHeroType.Signet)
                    {
                        OnPropertyChanged(nameof(ProgressText));
                        OnPropertyChanged(nameof(ProgressPercentage));
                    }
                };
            }

            _boolAdditionalStorage.CollectionChanged += OnVisitedPlacesChanged;
        }

        public void AddArtifactsTaken(List<Artifact> artifactsToPickFrom)
        {
            artifactsToPickFrom.ForEach(artifact =>
            {
                var artifactDisplay = AutoMapperConfig.Mapper.Map<ArtifactDisplay>(artifact);
                Artifacts.Add(artifactDisplay);
            });
        }

        public void AddArtifactsTakenOtherUsers(int artifactAmount)
        {
            ArtifactsAmountOtherUsers += artifactAmount;
        }

        public void ResetStorage(){
            BoolAdditionalStorage.Clear();
        }

        public void AddRoyalCard(RolayCard rolayCard)
        {
            PlayerRoyalCardManager.AddRolayCard(rolayCard);
            ResourceHeroManager.AddResource(ResourceHeroType.Siege, rolayCard.Siege);
            ResourceHeroManager.AddResource(ResourceHeroType.Army, rolayCard.Army);
            ResourceHeroManager.AddResource(ResourceHeroType.Magic, rolayCard.Magic);
            Points += rolayCard.ScorePoints;
        }

        public void SetBoolStorage(List<string> boolStorage){
            BoolAdditionalStorage.Clear();
            boolStorage.ForEach(b => BoolAdditionalStorage.Add(b));
        }

        public void SetCurrentHeroCard(CurrentHeroCard heroCard, HeroCardDisplay card, bool left)
        {
            PlayerHeroCardManager.SetCurrentHeroCard(heroCard);

            var cardResourceFrom = heroCard.ReplacedHeroCard != null? heroCard.ReplacedHeroCard.HeroCard: card;

            ResourceHeroManager.AddResource(ResourceHeroType.Siege, cardResourceFrom.Siege);
            ResourceHeroManager.AddResource(ResourceHeroType.Army, cardResourceFrom.Army);
            ResourceHeroManager.AddResource(ResourceHeroType.Magic, cardResourceFrom.Magic);
        }

        public void ResetCurrentHeroCard()
        {
            if (PlayerHeroCardManager.CurrentHeroCard == null) return;

            var changeHeroSides = PlayerAuraManager.AurasTypes.Any(a => a.Aura == AurasType.CHANGE_SIDES_OF_HERO_AFTER_PLAY);

            var card = PlayerHeroCardManager.CurrentHeroCard.HeroCard;
            if (changeHeroSides)
            {
                

                ResourceHeroManager.SubtractResource(ResourceHeroType.Siege, card.Siege);
                ResourceHeroManager.SubtractResource(ResourceHeroType.Army, card.Army);
                ResourceHeroManager.SubtractResource(ResourceHeroType.Magic, card.Magic);

                card = PlayerHeroCardManager.CurrentHeroCard.UnUsedHeroCard;

                ResourceHeroManager.AddResource(ResourceHeroType.Siege, card.Siege);
                ResourceHeroManager.AddResource(ResourceHeroType.Army, card.Army);
                ResourceHeroManager.AddResource(ResourceHeroType.Magic, card.Magic);

                PlayerHeroCardManager.ResetCurrentHeroCardReverse();
            }else{
                card = PlayerHeroCardManager.CurrentHeroCard.ReplacedHeroCard != null? PlayerHeroCardManager.CurrentHeroCard.ReplacedHeroCard.HeroCard: card;
            }
            

            ResourceHeroManager.AddResource(ResourceHeroType.Signet, card.RoyalSignet);
            Morale += card.Morale;

            PlayerHeroCardManager.ResetCurrentHeroCard();
        }



        public void AddMercenary(Mercenary mercenary)
        {
            var mercenaryDisplay = AutoMapperConfig.Mapper.Map<MercenaryDisplay>(mercenary);
            PlayerMercenariesManager.AddMercenary(mercenaryDisplay);
            PlayerMercenariesManager.SetProphecyCompleted(this);
            ResourceManager.AddGoldIncome(mercenary.IncomeGold);
            ResourceHeroManager.AddResource(ResourceHeroType.Siege, mercenary.Siege);
            ResourceHeroManager.AddResource(ResourceHeroType.Army, mercenary.Army);
            ResourceHeroManager.AddResource(ResourceHeroType.Magic, mercenary.Magic);
            Points += mercenary.ScorePoints;
        }

        public void ReceiveRewards(Reward reward)
        {
            ResourceManager.AddResources(reward.Resources);
            reward.AurasTypes.ForEach(auraType => PlayerAuraManager.AddAura(auraType));
            reward.HeroResources.ForEach(resource => ResourceHeroManager.AddResource(resource.Type, resource.Amount));
            reward.EndGameAura.ForEach(auraType => EndGameAuras.Add(auraType));

            if(reward.Morale > 0){
                Morale += reward.Morale.Value;
            }
        }


        public void PlayArtifact(Artifact artifact)
        {

            var artifactDisplay = AutoMapperConfig.Mapper.Map<ArtifactDisplay>(artifact);
            PlayerArtifactManager.YourArtifacts.Add(artifactDisplay);

            var artifactInHand = Artifacts.FirstOrDefault(a => a.InGameIndex == artifact.InGameIndex);
            if (artifactInHand != null)
            {
                Artifacts.Remove(artifactInHand);
            }
        }

        public void RerollArtifact(int artifactId, Artifact artifactRerolled)
        {
            var artifact = Artifacts.FirstOrDefault(a => a.InGameIndex == artifactId);

            BoolAdditionalStorage.Add(BoolHelper.EXTRA_REROLL_PLAYED);

            if (artifact != null)
            {
                Artifacts.Remove(artifact);
                var artifactDisplay = AutoMapperConfig.Mapper.Map<ArtifactDisplay>(artifactRerolled);
                Artifacts.Add(artifactDisplay);
            }
        }


        public double ProgressPercentage => (double)ResourceHeroManager.GetResourceAmount(ResourceHeroType.Signet) / PlayerRoyalCardManager.SignetsNeededForNextCard * 100;
        public string ProgressText => $"{ResourceHeroManager.GetResourceAmount(ResourceHeroType.Signet)} / {PlayerRoyalCardManager.SignetsNeededForNextCard}";

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}