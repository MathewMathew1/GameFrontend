using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BoardGameFrontend.Models;
using BoardGameFrontend.Commands;
using BoardGameFrontend.AutoMapper;
using System;
using BoardGameFrontend.Services;
using System.Collections.Specialized;

namespace BoardGameFrontend.Managers
{
    public class ArtifactManager : INotifyPropertyChanged
    {
        public ObservableCollection<ArtifactDisplay> ArtifactsToPickFrom { get; set; } = new ObservableCollection<ArtifactDisplay>();

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

        private string _pickText;
        public string PickText
        {
            get => _pickText;
            private set
            {
                _pickText = value;
                OnPropertyChanged(nameof(PickText));
            }
        }

        private void ArtifactsToPickFrom_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePickText();
        }

        private void UpdatePickText()
        {
            int countToPick = ArtifactsToPickFrom.Count - 1;
            PickText = $"Pick {countToPick} Artifact{(countToPick > 1 ? "s" : "")}";
        }

        private int _numberOfArtifacts;
        public int NumberOfArtifacts
        {
            get => _numberOfArtifacts;
            set
            {
                _numberOfArtifacts = value;

                OnPropertyChanged(nameof(NumberOfArtifacts));
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

        private int _numberOfTossedArtifacts;
        public int NumberOfTossedArtifacts
        {
            get => _numberOfTossedArtifacts;
            set
            {
                _numberOfTossedArtifacts = value;

                OnPropertyChanged(nameof(NumberOfTossedArtifacts));
            }
        }

        private int _numberOfLeftArtifacts;
        public int NumberOfLeftArtifacts
        {
            get => _numberOfLeftArtifacts;
            set
            {
                _numberOfLeftArtifacts = value;

                OnPropertyChanged(nameof(NumberOfLeftArtifacts));
            }
        }

        private bool _selectedCorrectNumberOfArtifacts;
        public bool SelectedCorrectNumberOfArtifacts
        {
            get => _selectedCorrectNumberOfArtifacts;
            set
            {
                _selectedCorrectNumberOfArtifacts = value;
                OnPropertyChanged(nameof(SelectedCorrectNumberOfArtifacts));
            }
        }

        private int _selectedArtifactsCount; // New field to track selected artifacts
        public int SelectedArtifactsCount
        {
            get => _selectedArtifactsCount;
            set
            {
                _selectedArtifactsCount = value;
                CheckIfSelectedCorrectNumberOfArtifacts();
                OnPropertyChanged(nameof(SelectedArtifactsCount));
            }
        }

        private Game _game;

        public ICommand SelectArtifactCommand { get; private set; }
        public ICommand SelectArtifactToPlayCommand { get; private set; }
        public ICommand PlayArtifactCommand { get; private set; }
        public ICommand RerollArtifactCommand { get; private set; }
        public ICommand TakeArtifactsCommand { get; private set; }
        public ICommand CancelChoseOneCommand { get; private set; }
        public ICommand PlayArtifactWithChooseOneCommand { get; private set; }
        public ICommand SetChooseOneCommand { get; private set; }

        public ArtifactManager(Game game)
        {
            _game = game;
            SelectArtifactCommand = new RelayCommandWithTypes<int>(SelectArtifactById);
            SelectArtifactToPlayCommand = new RelayCommandWithTypes<int>(SelectArtifactToPlay);
            TakeArtifactsCommand = new RelayCommandWithTypes<ServiceLobbyWindow>(TakeArtifacts);
            PlayArtifactCommand = new RelayCommandWithTypes<ServiceLobbyWindow>(PlayArtifact);
            PlayArtifactWithChooseOneCommand = new RelayCommandWithTypes<object>(PlayArtifactWithChooseOne);
            RerollArtifactCommand = new RelayCommandWithTypes<ServiceLobbyWindow>(RerollArtifact);
            CancelChoseOneCommand = new RelayCommand(async async => CloseChooseOne());
            SetChooseOneCommand = new RelayCommand(async async => CloseChooseOne());
            ArtifactsToPickFrom.CollectionChanged += ArtifactsToPickFrom_CollectionChanged;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetArtifactData(ArtifactInfo artifactInfo)
        {
            foreach (var artifact in artifactInfo.ArtifactToPickFrom)
            {
                var artifactDisplay = AutoMapperConfig.Mapper.Map<ArtifactDisplay>(artifact);
                ArtifactsToPickFrom.Add(artifactDisplay);
            }
            NumberOfLeftArtifacts = artifactInfo.ArtifactsLeftAmount;
            NumberOfTossedArtifacts = artifactInfo.ArtifactsTossedAwayAmount;
        }

        public void SetArtifactsToPickFrom(List<Artifact> artifacts)
        {
            foreach (var artifact in artifacts)
            {
                var artifactDisplay = AutoMapperConfig.Mapper.Map<ArtifactDisplay>(artifact);
                ArtifactsToPickFrom.Add(artifactDisplay);
            }
        }

        public void SetArtifacts(ArtifactToPickFromData newArtifacts)
        {
            ArtifactsToPickFrom.Clear();
            foreach (var artifact in newArtifacts.Artifacts)
            {
                var artifactDisplay = AutoMapperConfig.Mapper.Map<ArtifactDisplay>(artifact);
                ArtifactsToPickFrom.Add(artifactDisplay);
            }
            NumberOfArtifacts = newArtifacts.Artifacts.Count;
            NumberOfTossedArtifacts = newArtifacts.ArtifactsLeftTossed;
            NumberOfLeftArtifacts = newArtifacts.ArtifactsLeft;
            SelectedArtifactsCount = 0;
            OnPropertyChanged(nameof(ArtifactsToPickFrom));
        }

        public void SetArtifacts(ArtifactToPickFromDataForOtherUsers newArtifacts)
        {
            ArtifactsToPickFrom.Clear();

            NumberOfArtifacts = newArtifacts.ArtifactsAmount;
            NumberOfTossedArtifacts = newArtifacts.ArtifactsLeftTossed;
            NumberOfLeftArtifacts = newArtifacts.ArtifactsLeft;
            SelectedArtifactsCount = 0;

            UpdatePlayableArtifacts(_game.UserControlledPlayer);
            OnPropertyChanged(nameof(ArtifactsToPickFrom));
        }

        public void SetData(ArtifactTakenData newArtifacts)
        {
            ArtifactsToPickFrom.Clear();

            NumberOfTossedArtifacts = newArtifacts.ArtifactsLeftTossed;
            NumberOfLeftArtifacts = newArtifacts.ArtifactsLeft;

            OnPropertyChanged(nameof(ArtifactsToPickFrom));
        }

        public void SetData(ArtifactTakenDataForOtherUsers newArtifacts)
        {
            ArtifactsToPickFrom.Clear();

            NumberOfTossedArtifacts = newArtifacts.ArtifactsLeftTossed;
            NumberOfLeftArtifacts = newArtifacts.ArtifactsLeft;

            OnPropertyChanged(nameof(ArtifactsToPickFrom));
        }

        public void SelectArtifactById(int id)
        {
            var artifact = ArtifactsToPickFrom.FirstOrDefault(a => a.InGameIndex == id);
            if (artifact != null)
            {
                artifact.Selected = !artifact.Selected;

                SelectedArtifactsCount += artifact.Selected ? 1 : -1;
            }
        }



        public void SelectArtifactToPlay(int id)
        {
            var artifact = _game.UserControlledPlayer.Artifacts.FirstOrDefault(a => a.InGameIndex == id);
            if (SelectedArtifact != null)
            {
                SelectedArtifact.Selected = false;
            }

            if (artifact != null)
            {
                ShowChooseOne = false;
                SelectedArtifact = artifact;
                SelectedArtifact.Selected = true;
            }
        }

        public void RerollArtifact(ServiceLobbyWindow serviceLobbyWindow)
        {
            serviceLobbyWindow.RerollArtifact(SelectedArtifact.InGameIndex);
            ShowChooseOne = false;
            SelectedArtifact.Selected = false;
            SelectedArtifact = null;
        }

        public void PlayArtifact(ServiceLobbyWindow serviceLobbyWindow)
        {
            if (SelectedArtifact.Effect2 == -1)
            {
                if (!CanPlayArtifact(SelectedArtifact.Effect1, _game.UserControlledPlayer))
                {
                    return;
                }
                serviceLobbyWindow.PlayArtifact(SelectedArtifact.InGameIndex, true, false);
                CleanData();
                return;
            }

            var canPlaySecondOption = CanPlayArtifact(SelectedArtifact.Effect2, _game.UserControlledPlayer);
            if (SelectedArtifact.SecondEffectSuperior && canPlaySecondOption)
            {
                serviceLobbyWindow.PlayArtifact(SelectedArtifact.InGameIndex, false, false);
                CleanData();
                return;
            }

            if (!canPlaySecondOption)
            {
                serviceLobbyWindow.PlayArtifact(SelectedArtifact.InGameIndex, true, false);
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

        private bool CanPlayArtifact(int effectId, PlayerInGameViewModel player)
        {
            var req = EffectsFactory.GetReqById(effectId);
            if (req == -1 || req == null) return true;

            var requirement = RequirementMovementStore.GetRequirementById(req.Value);
            bool fulfillThisRequirement = requirement.CheckRequirements(player);
            return fulfillThisRequirement;
        }

        public void PlayArtifactWithChooseOne(object parameter)
        {
            var tuple = parameter as Tuple<object, object>;
            if (tuple != null)
            {
                var firstEffect = (bool)tuple.Item2;
                var lobbyService = tuple.Item1 as ServiceLobbyWindow;

                if (lobbyService != null)
                {
                    if (SelectedArtifact.Effect2 != -1)
                    {
                        lobbyService.PlayArtifact(SelectedArtifact.InGameIndex, firstEffect, false);
                        ShowChooseOne = false;
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
                artifact.ArtifactPlayProperties.CanPlayEffectFirst = CanPlayArtifact(artifact.Effect1, player);
                artifact.ArtifactPlayProperties.CanPlayEffectSecond = CanPlayArtifact(artifact.Effect2, player);
                artifact.ArtifactPlayProperties.CanPlayCard = artifact.ArtifactPlayProperties.CanPlayEffectFirst || (artifact.ArtifactPlayProperties.ThereIsSecondEffect && artifact.ArtifactPlayProperties.CanPlayEffectSecond);
            }

        }

        public void CloseChooseOne()
        {
            ShowChooseOne = false;
            SelectedArtifact.Selected = false;
            SelectedArtifact = null;
        }

        private void TakeArtifacts(ServiceLobbyWindow serviceLobbyWindow)
        {
            var listOfArtifacts = new List<int>();
            foreach (var artifact in ArtifactsToPickFrom)
            {
                if (artifact.Selected)
                {
                    listOfArtifacts.Add(artifact.InGameIndex);
                }
            }
            serviceLobbyWindow.TakeArtifacts(listOfArtifacts);
        }

        public void CheckIfSelectedCorrectNumberOfArtifacts()
        {
            if (ArtifactsToPickFrom.Count == 0)
            {
                SelectedCorrectNumberOfArtifacts = false;
            }
            SelectedCorrectNumberOfArtifacts = SelectedArtifactsCount == ArtifactsToPickFrom.Count - 1;
        }
    }
}
