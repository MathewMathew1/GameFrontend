using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BoardGameFrontend.Models;
using BoardGameFrontend.Commands;
using System;
using BoardGameFrontend.Services;
using System.Threading.Tasks;
using BoardGameFrontend.AutoMapper;

namespace BoardGameFrontend.Managers
{
    public class PlayerArtifactManager : INotifyPropertyChanged
    {
        public ObservableCollection<ArtifactDisplay> YourArtifacts { get; set; } = new ObservableCollection<ArtifactDisplay>();
        public PlayerInGameViewModel _player {get; set;}

        private ArtifactDisplay? _selectedArtifact;
        public ArtifactDisplay? SelectedArtifact
        {
            get => _selectedArtifact;
            set
            {
                _selectedArtifact = value;

                OnPropertyChanged(nameof(SelectedArtifact));
            }
        }

        private bool _showChoose;
        public bool ShowChooseOne
        {
            get => _showChoose;
            set
            {
                _showChoose = value;

                OnPropertyChanged(nameof(ShowChooseOne));
            }
        }


        public ICommand SelectArtifactCommand { get; private set; }
        public ICommand PlayArtifactCommand { get; private set; }
        public ICommand CancelChoseOneCommand { get; private set; }
        public ICommand PlayArtifactWithChooseOneCommand { get; private set; }
        public ICommand SetChooseOneCommand { get; private set; }

        public PlayerArtifactManager(PlayerInGameViewModel player)
        {
            _player = player;
            SelectArtifactCommand = new RelayCommandWithTypes<int>(SelectArtifactById);
            PlayArtifactCommand = new AsyncRelayCommandWithTypes<ServiceLobbyWindow>(RePlayArtifact);
            PlayArtifactWithChooseOneCommand = new AsyncRelayCommandWithTypes<object>(PlayArtifactWithChooseOne);
            CancelChoseOneCommand = new RelayCommand(async async => CloseChooseOne());
            SetChooseOneCommand = new RelayCommand(async async => CloseChooseOne());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void AddArtifactsPlayed(List<Artifact> artifactsToPickFrom)
        {
            artifactsToPickFrom.ForEach(artifact =>
            {
                var artifactDisplay = AutoMapperConfig.Mapper.Map<ArtifactDisplay>(artifact);
                YourArtifacts.Add(artifactDisplay);
            });
        }

        public void SelectArtifactById(int id)
        {
            if(SelectedArtifact != null){
                SelectedArtifact.Selected = false;
                SelectedArtifact = null;
            }

            var artifact = YourArtifacts.FirstOrDefault(a => a.InGameIndex == id);
            if (artifact != null)
            {
                artifact.Selected = true;
                SelectedArtifact = artifact;
            }
        }

        public async Task RePlayArtifact(ServiceLobbyWindow serviceLobbyWindow)
        {
            if(SelectedArtifact == null) return;

            if (SelectedArtifact.Effect2 == -1)
            {
                if (!CanPlayArtifact(SelectedArtifact.Effect1))
                {
                    return;
                }
                
                await serviceLobbyWindow.PlayArtifact(SelectedArtifact.InGameIndex, true, true);
                CleanData();    
                return;
            }

            var canPlaySecondOption = CanPlayArtifact(SelectedArtifact.Effect2);
            if (SelectedArtifact.SecondEffectSuperior && canPlaySecondOption)
            {
                
                await serviceLobbyWindow.PlayArtifact(SelectedArtifact.InGameIndex, false, true);
                CleanData();
                return;
            }

            if (!canPlaySecondOption)
            {
                await serviceLobbyWindow.PlayArtifact(SelectedArtifact.InGameIndex, true, true);      
                CleanData();       
                return;
            }

            ShowChooseOne = true;
        }

        private void CleanData()
        {
            ShowChooseOne = false;
            SelectedArtifact.Selected = false;
            SelectedArtifact = null;
        }

        private bool CanPlayArtifact(int effectId)
        {
            var req = EffectsFactory.GetReqById(effectId);
            if (req == -1 || req == null) return true;

            var requirement = RequirementMovementStore.GetRequirementById(req.Value);
            bool fulfillThisRequirement = requirement!.CheckRequirements(_player);
            return fulfillThisRequirement;
        }

        public async Task PlayArtifactWithChooseOne(object parameter)
        {
            if(SelectedArtifact == null) return;

            var tuple = parameter as Tuple<object, object>;
            if (tuple != null)
            {
                var firstEffect = (bool)tuple.Item2;
                var lobbyService = tuple.Item1 as ServiceLobbyWindow;

                if (lobbyService != null)
                {
                    if (SelectedArtifact.Effect2 != -1)
                    {
                        ShowChooseOne = false;
                        await lobbyService.PlayArtifact(SelectedArtifact.InGameIndex, firstEffect, true);
                        SelectedArtifact.Selected = false;
                        SelectedArtifact = null;
                        return;
                    }
                }
            }

        }

        public void UpdatePlayableArtifacts(PlayerInGameViewModel player)
        {
            foreach (var artifact in player.Artifacts)
            {
                artifact.ArtifactPlayProperties.ThereIsSecondEffect = artifact.Effect2 != -1;
                artifact.ArtifactPlayProperties.CanPlayEffectFirst = CanPlayArtifact(artifact.Effect1);
                artifact.ArtifactPlayProperties.CanPlayEffectSecond = CanPlayArtifact(artifact.Effect2);
                artifact.ArtifactPlayProperties.CanPlayCard = artifact.ArtifactPlayProperties.CanPlayEffectFirst || (artifact.ArtifactPlayProperties.ThereIsSecondEffect && artifact.ArtifactPlayProperties.CanPlayEffectSecond);
            }

        }

        public void CloseChooseOne()
        {
            ShowChooseOne = false;
            SelectedArtifact.Selected = false;
            SelectedArtifact = null;
        }


    }
}
