using System;
using System.ComponentModel;
using System.Linq;
using BoardGameFrontend.Models;
using BoardGameFrontend.VisualManager;
using BoardGameFrontend.AutoMapper;
using System.Collections.Generic;



namespace BoardGameFrontend.Managers
{
    public class Game : INotifyPropertyChanged
    {
        public TurnManager TurnManager { get; set; }
        public TokenManager TokenManager { get; set; }
        public PlayerManager PlayerManager { get; set; }
        public GameVisualManager GameVisualManager { get; set; }
        public PawnManager PawnManager { get; set; }
        public MercenaryManager MercenaryManager { get; set; }
        public MiniPhaseManager MiniPhaseManager { get; set; }
        public PhaseManager PhaseManager { get; set; }
        public HeroCardsBoardManager HeroCardsBoardManager { get; set; } = new HeroCardsBoardManager();
        public ArtifactManager ArtifactManager { get; set; }
        public RolayCardsManager RolayCardsManager { get; set; } = new RolayCardsManager();

        private Guid _playerId;
        public Guid PlayerId
        {
            get => _playerId;
            set
            {
                if (_playerId != value)
                {
                    _playerId = value;
                    OnPropertyChanged(nameof(PlayerId));
                    OnPropertyChanged(nameof(UserControlledPlayer)); // Trigger UI update
                }
            }
        }

        private PlayerInGameViewModel _currentPlayer;
        public PlayerInGameViewModel CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                if (_currentPlayer != value)
                {
                    _currentPlayer = value;
                    OnPropertyChanged(nameof(CurrentPlayer));
                }
            }
        }

        private bool _isUserControlledPlayersTurn;
        public bool IsUserControlledPlayersTurn
        {
            get => _isUserControlledPlayersTurn;
            set
            {
                if (_isUserControlledPlayersTurn != value)
                {
                    _isUserControlledPlayersTurn = value;
                    OnPropertyChanged(nameof(IsUserControlledPlayersTurn));
                }
            }
        }

        public PlayerInGameViewModel UserControlledPlayer
        {
            get => PlayerManager.Players.FirstOrDefault(p => p.Id == PlayerId)!;
            set { }
        }


        private bool _gameHasStarted;
        public bool GameHasStarted
        {
            get => _gameHasStarted;
            set
            {
                _gameHasStarted = value;
                OnPropertyChanged(nameof(GameHasStarted));
            }
        }

        public Game()
        {
            GameVisualManager = new GameVisualManager(this);
            PawnManager = new PawnManager(GameVisualManager.GameTiles.GetTileById(27), this);
            PlayerId = new Guid();
            ArtifactManager = new ArtifactManager(this);
            GameHasStarted = false;
            TurnManager = new TurnManager();
            TokenManager = new TokenManager(this);
            PhaseManager = new PhaseManager(this);
            PlayerManager = new PlayerManager();
            MercenaryManager = new MercenaryManager(this);
            MiniPhaseManager = new MiniPhaseManager(this);
            PlayerManager.Players.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(UserControlledPlayer));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateCurrentTurn(int currentTurn, int roundCount)
        {
            TurnManager.TurnCount = currentTurn;
            TurnManager.RoundCount = roundCount;
        }

        public void UpdateCurrentPlayer(Guid userId, bool? newPhase = true)
        {
            if (CurrentPlayer != null)
            {
                CurrentPlayer.TimerManager.StopTimer();
            }

            if (newPhase != null && !newPhase.Value && CurrentPlayer != null)
            {
                CurrentPlayer.AlreadyPlayedCurrentPhase = true;
            }

            for (int i = 0; i < PlayerManager.Players.Count; i++)
            {
                PlayerManager.Players[i].IsCurrentPlayer = false;
                if (PlayerManager.Players[i].Id == userId)
                {
                    TurnManager.CurrentPlayerIndex = i;
                    PlayerManager.Players[i].IsCurrentPlayer = true;
                    CurrentPlayer = PlayerManager.Players[i];
                    CurrentPlayer.TimerManager.StartTimer();
                    if (PlayerManager.Players[i].Id == PlayerId)
                    {
                        IsUserControlledPlayersTurn = true;
                    }
                    else
                    {
                        IsUserControlledPlayersTurn = false;
                    }
                }
            }

            if (PhaseManager.CurrentPhase.Name == PhaseType.BoardPhase && CurrentPlayer.PlayerHeroCardManager.CurrentHeroCard != null)
            {
                CurrentPlayer.PlayerHeroCardManager.CurrentHeroCard.VisitedPlaces.Add(PawnManager.CurrentTile.Id);
            }
        }

        public void UpdateBuyableThings()
        {
            MercenaryManager.SetCanBuyForAllMercenaries();
        }

        public void StartGame(StartOfGame startOfGame)
        {
            var playerInGameList = startOfGame.Players.Select(data => AutoMapperConfig.Mapper.Map<PlayerInGameViewModel>(data)).ToList();
            PlayerManager.SetPlayers(playerInGameList);
            MercenaryManager.SetData(startOfGame.MercenaryData);
            RolayCardsManager.SetRolayCards(startOfGame.RolayCards);
            GameHasStarted = true;
            UpdateCurrentPlayer(startOfGame.Players[0].Id);
            CurrentPlayer.AlreadyPlayedCurrentPhase = true;
            UpdateCurrentTurn(1, 1);

            GameVisualManager.GameTiles.AddTokensToTiles(startOfGame.TokenSetup);

            SetupVisualUpdates();
        }

        public void SetupVisualUpdates()
        {
            var player = PlayerManager.Players.FirstOrDefault(p => p.Id == PlayerId);

            if (player != null)
            {
                PlayerManager.SelectedPlayer = player;
                UserControlledPlayer = player;
                UserControlledPlayer.ResourceManager.UpdateBuyableThings = () => MercenaryManager.SetCanBuyForAllMercenaries();

                foreach (var _player in PlayerManager.Players)
                {
                    _player.ResourceHeroManager.UpdateBuyableThings = () =>
                    {
                        _player.PlayerMercenariesManager.SetProphecyCompleted(_player);
                    };

                    _player.PlayerHeroCardManager.UpdateBuyableThings = () =>
                    {
                        _player.PlayerMercenariesManager.SetProphecyCompleted(_player);
                    };

                }

                UserControlledPlayer.ResourceHeroManager.UpdateBuyableThings = () =>
                {
                    MercenaryManager.SetCanBuyForAllMercenaries();
                    MercenaryManager.SetProphecyCompleted(UserControlledPlayer);
                    ArtifactManager.UpdatePlayableArtifacts(UserControlledPlayer);
                };

                UserControlledPlayer.PlayerAuraManager.UpdateBuyableThings = () =>
                {
                    MercenaryManager.SetCanBuyForAllMercenaries();
                };

                UserControlledPlayer.PlayerHeroCardManager.UpdateBuyableThings = () =>
                {
                    MercenaryManager.SetCanBuyForAllMercenaries();
                    MercenaryManager.SetProphecyCompleted(UserControlledPlayer);
                    UserControlledPlayer.PlayerMercenariesManager.SetProphecyCompleted(UserControlledPlayer);
                };
            }
        }

        public void MercenaryPicked(MercenaryPickedData mercenaryPickedData)
        {
            var player = PlayerManager.GetPlayerById(mercenaryPickedData.Player.Id);
            mercenaryPickedData.ResourcesSpend.ForEach(resource =>
            {
                if (player != null)
                {
                    player.ResourceManager.SubtractResource(resource.Name, resource.Amount);
                }
            });
            MercenaryManager.RemoveMercenaryByGameIndex(mercenaryPickedData.Card.InGameIndex);

            if(mercenaryPickedData.MercenaryReplacement != null){
                MercenaryManager.AddNewBuyableMercenary(mercenaryPickedData.MercenaryReplacement);
            }

            MercenaryManager.ChangeMercenariesAmountData(mercenaryPickedData.MercenariesLeftData);

            if (player != null)
            {
                player.AddMercenary(mercenaryPickedData.Card);
                if (mercenaryPickedData.Card.Morale != 0)
                {
                    PlayerManager.AddMoraleToPlayer(player, mercenaryPickedData.Card.Morale);
                }
                if (mercenaryPickedData.Reward != null)
                {
                    player.ReceiveRewards(mercenaryPickedData.Reward);
                }
            }
        }

        public void EndOfTurn(Guid playerId)
        {
            PlayerManager.EndTurn(playerId);
        }

        public void EndOfTurn(EndOfTurnEventData data)
        {
            PlayerManager.ResetAllPlayersPlayedTurn();
            TurnManager.TurnCount = data.TurnCount;
        }

        public void EndOfRound(EndOfRoundData endOfRoundData)
        {
            PlayerManager.EndOfRound();
            MercenaryManager.EndOfRound(endOfRoundData.EndOfRoundMercenaryData);
        }

        public void MoveOnTile(MoveOnTileData moveOnTileData)
        {
            var player = PlayerManager.GetPlayerById(moveOnTileData.Player.Id);
            if (player == null) return;

            if (player.PlayerHeroCardManager.CurrentHeroCard != null)
            {
                player.PlayerHeroCardManager.CurrentHeroCard.MovementFullLeft = moveOnTileData.MovementFullLeft;
                player.PlayerHeroCardManager.CurrentHeroCard.MovementUnFullLeft = moveOnTileData.MovementUnFullLeft;
                player.PlayerHeroCardManager.CurrentHeroCard.VisitedPlaces.Add(moveOnTileData.TileId);
            }

            if (moveOnTileData.TileReward.Artifact != null)
            {
                player.AddArtifactsTaken(new List<Artifact> { moveOnTileData.TileReward.Artifact });
            }
            else if (moveOnTileData.TileReward.GotArtifact)
            {
                player.AddArtifactsTakenOtherUsers(1);
            }

            player.ResourceManager.AddResources(moveOnTileData.TileReward.Resources);
            if (moveOnTileData.TileReward.RerollMercenaryAction == true)
            {
                MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.MercenaryRerollPhase });
            }

            if (moveOnTileData.TileReward.TokenReward != null)
            {
                player.ReceiveRewards(moveOnTileData.TileReward.TokenReward.Reward);
                var token = GameVisualManager.GameTiles.RemoveTokenFromTile(moveOnTileData.TileId);
                if (token != null)
                {
                    player.TokenManager.AddToken(token);
                }
            }

            if (moveOnTileData.TileReward.TempSignet == true)
            {
                player.PlayerAuraManager.AddAura(new AuraTypeWithLongevity { Permanent = false, Aura = AurasType.TEMPORARY_SIGNET });
                player.ResourceHeroManager.AddResource(ResourceHeroType.Signet, 1);
            }

            if(!moveOnTileData.AdjacentMovement){
                PawnManager.MoveToTile(GameVisualManager.GameTiles.GetTileById(moveOnTileData.TileId));
            }else{
                PawnManager.CancelMovement();
            }
            
            PawnManager.CheckForTeleport(moveOnTileData.TileReward.TeleportedTileId);

        }

        public void TileReward(TileRewardData tileRewardData)
        {
            var player = PlayerManager.GetPlayerById(tileRewardData.Player.Id);
            if (player == null) return;

            if (tileRewardData.TileReward.Artifact != null)
            {
                player.AddArtifactsTaken(new List<Artifact> { tileRewardData.TileReward.Artifact });
            }
            else if (tileRewardData.TileReward.GotArtifact)
            {
                player.AddArtifactsTakenOtherUsers(1);
            }

            if (tileRewardData.TileReward.TokenReward != null)
            {
                player.ReceiveRewards(tileRewardData.TileReward.TokenReward.Reward);

            }

            if (tileRewardData.TileReward.TempSignet == true)
            {
                player.PlayerAuraManager.AddAura(new AuraTypeWithLongevity { Permanent = false, Aura = AurasType.TEMPORARY_SIGNET });
                player.ResourceHeroManager.AddResource(ResourceHeroType.Signet, 1);
            }

            player.ResourceManager.AddResources(tileRewardData.TileReward.Resources);
            if (tileRewardData.TileReward.RerollMercenaryAction == true)
            {
                MiniPhaseManager.SetCurrentPhase(new MiniPhase { Name = MiniPhaseType.MercenaryRerollPhase });
            }
        }

        public void PlayArtifact(ArtifactPlayed artifactPlayed)
        {
            var player = PlayerManager.GetPlayerById(artifactPlayed.Player.Id);
            if (player == null) return;

            player.ReceiveRewards(artifactPlayed.Reward);
            player.PlayArtifact(artifactPlayed.Artifact);
        }

        public void RePlayArtifact(ArtifactPlayed artifactPlayed)
        {
            var player = PlayerManager.GetPlayerById(artifactPlayed.Player.Id);
            if (player == null) return;

            player.ReceiveRewards(artifactPlayed.Reward);

        }

        public void AddPossibleArtifacts(ArtifactToPickFromData artifactToPickFrom)
        {
            ArtifactManager.SetArtifacts(artifactToPickFrom);
        }

        public void AddArtifacts(ArtifactToPickFromData artifactData)
        {
            var player = PlayerManager.GetPlayerById(artifactData.Player.Id);
            if (player == null) return;

            player.AddArtifactsTaken(artifactData.Artifacts);
            ArtifactManager.NumberOfLeftArtifacts = artifactData.ArtifactsLeft;
            ArtifactManager.NumberOfTossedArtifacts = artifactData.ArtifactsLeftTossed;
            ArtifactManager.UpdatePlayableArtifacts(player);
        }

        public void AddArtifacts(ArtifactToPickFromDataForOtherUsers artifactData)
        {

            var player = PlayerManager.GetPlayerById(artifactData.Player.Id);
            if (player == null) return;

            player.AddArtifactsTakenOtherUsers(artifactData.ArtifactsAmount);
            ArtifactManager.NumberOfLeftArtifacts = artifactData.ArtifactsLeft;
            ArtifactManager.NumberOfTossedArtifacts = artifactData.ArtifactsLeftTossed;
        }

        public void AddPossibleArtifacts(ArtifactToPickFromDataForOtherUsers artifactToPickFrom)
        {
            ArtifactManager.SetArtifacts(artifactToPickFrom);
        }

        public void AddTakenArtifacts(ArtifactTakenData artifactsTaken)
        {
            var player = PlayerManager.GetPlayerById(artifactsTaken.Player.Id);
            if (player == null) return;

            ArtifactManager.SetData(artifactsTaken);
            player.AddArtifactsTaken(artifactsTaken.Artifacts);
            ArtifactManager.UpdatePlayableArtifacts(player);
        }

        public void AddTakenArtifacts(ArtifactTakenDataForOtherUsers artifactsTaken)
        {
            var player = PlayerManager.GetPlayerById(artifactsTaken.Player.Id);
            if (player == null) return;

            ArtifactManager.SetData(artifactsTaken);
            player.AddArtifactsTakenOtherUsers(artifactsTaken.ArtifactsAmount);
        }

        public void MoveByAura(MoveOnTileOnEvent moveOnTileOnEvent)
        {
            var player = PlayerManager.GetPlayerById(moveOnTileOnEvent.Player.Id);

            if (player == null) return;

            PawnManager.MoveToTile(GameVisualManager.GameTiles.GetTileById(moveOnTileOnEvent.TileId));

            if (moveOnTileOnEvent.AuraUsed != null)
            {
                player.PlayerAuraManager.RemoveAurasOfTypeAndReturnAmountCount(moveOnTileOnEvent.AuraUsed.Value);
            }

        }

        public void RefreshBuyableMercenaries(BuyableMercenariesRefreshed buyableMercenariesRefreshed)
        {
            MercenaryManager.ChangeMercenariesAmountData(buyableMercenariesRefreshed.MercenariesLeftData);
            MercenaryManager.SetMercenaries(buyableMercenariesRefreshed.NewBuyableMercenaries);
        }

        public void Teleport(TeleportationData teleportationData)
        {
            PawnManager.MoveToTile(GameVisualManager.GameTiles.GetTileById(teleportationData.TileId));
            MiniPhaseManager.SetCurrentPhase(null);
        }

        public void RerollArtifact(ArtifactRerolledData artifactRerolledData)
        {
            var player = PlayerManager.GetPlayerById(artifactRerolledData.Player.Id);
            if (player == null) return;

            player.RerollArtifact(artifactRerolledData.Artifact.InGameIndex, artifactRerolledData.ArtifactRerolled);
        }

        public void FulfillProphecy(FulfillProphecy fulfillProphecy)
        {
            var player = PlayerManager.GetPlayerById(fulfillProphecy.PlayerId);
            if (player == null) return;

            player.PlayerMercenariesManager.SetAlwaysCompleteProphecy(fulfillProphecy.MercenaryId);
            player.PlayerAuraManager.RemoveAuraOfType(AurasType.FULFILL_PROPHECY);
        }

        public void AddAura(AddAura addAura)
        {
            var player = PlayerManager.GetPlayerById(addAura.PlayerId);
            if (player == null) return;

            player.PlayerAuraManager.AddAura(addAura.Aura);
        }

        public void LockMercenary(LockMercenaryData lockMercenaryData)
        {
            MercenaryManager.LockMercenary(lockMercenaryData);
        }

        public void BlockTile(BlockedTileData blockedTileData)
        {
            GameVisualManager.GameTiles.BlockTile(blockedTileData.TileId, blockedTileData.Token);
            GameVisualManager.TilesBorderManager.RemoveAvailableConnections();
        }

        public void RotatePawn(RotateTileEventData data)
        {
            PawnManager.MoveToTile(GameVisualManager.GameTiles.GetTileById(data.TileId));
        }

        public void RoyalCardPlayed(RoyalCardPlayed royalCardPlayed)
        {
            var player = PlayerManager.GetPlayerById(royalCardPlayed.PlayerId);
            if (player == null) return;

            player.AddRoyalCard(royalCardPlayed.RoyalCard);
            if (royalCardPlayed.RoyalCard.Morale > 0)
            {
                PlayerManager.AddMoraleToPlayer(player, royalCardPlayed.RoyalCard.Morale);
            }
            if (royalCardPlayed.Reward != null)
            {
                player.ReceiveRewards(royalCardPlayed.Reward);
            }
            player.PlayerRoyalCardManager.SetSignetsNeededForNextCard(royalCardPlayed.AmountOfSignetsForNextRoyalCard);

            var playerView = AutoMapperConfig.Mapper.Map<PlayerViewModel>(player);
            RolayCardsManager.SetRolayCardTaken(playerView, royalCardPlayed.RoyalCard.Id);
        }

        public void ReplacedNextHeroCardPlayed(ReplaceNextHeroEventData data)
        {
            var player = PlayerManager.GetPlayerById(data.PlayerId);
            if (player == null) return;

            player.PlayerAuraManager.AddAura(data.ReplacementHeroAura);
        }

        public void GoldIntoMovementEvent(GoldIntoMovementEventData data)
        {
            var player = PlayerManager.GetPlayerById(data.PlayerId);
            if (player == null) return;

            player.PlayerAuraManager.RemoveAuraOfType(AurasType.GOLD_FOR_MOVEMENT);
            if (player.PlayerHeroCardManager.CurrentHeroCard == null) return;

            player.ResourceManager.SubtractResource(ResourceType.Gold, 2);
            player.PlayerHeroCardManager.CurrentHeroCard.MovementFullLeft = data.MovementFullLeft;
        }

        public void NewTokens(NewTokensSetupEventData data){
            GameVisualManager.GameTiles.AddTokensToTiles(data.NewTokens);
        }

        public void PreMercenaryReroll(PreMercenaryRerolled data){
            MercenaryManager.PreMercenaryRerolled(data);
        }

        public void MovementsConvertedEvent(FullMovementIntoEmptyEventData data)
        {
            var player = PlayerManager.GetPlayerById(data.PlayerId);
            if (player == null) return;

            player.PlayerAuraManager.RemoveAuraOfType(AurasType.FULL_MOVEMENT_INTO_EMPTY);
            if (player.PlayerHeroCardManager.CurrentHeroCard == null) return;

            player.PlayerHeroCardManager.CurrentHeroCard.MovementFullLeft = data.MovementFullLeft;
            player.PlayerHeroCardManager.CurrentHeroCard.MovementUnFullLeft = data.MovementUnFullLeft;
        }

       public void SetData(FullGameData fullGameData)
        {
            var playerInGameList = fullGameData.PlayersData.Select(data => AutoMapperConfig.Mapper.Map<PlayerInGameViewModel>(data.Player)).ToList();
            
            PawnManager.MoveToTile(GameVisualManager.GameTiles.GetTileById(fullGameData.PawnTilePosition));    
            PlayerManager.SetPlayers(playerInGameList);
            MercenaryManager.SetData(fullGameData.MercenaryData);
            RolayCardsManager.SetRolayCards(fullGameData.RoyalCards);
            HeroCardsBoardManager.SetHeroCards(fullGameData.HeroCards);
            ArtifactManager.SetArtifactData(fullGameData.ArtifactInfo);
            GameHasStarted = true;
            UpdateCurrentPlayer(fullGameData.CurrentPlayerId);
            UpdateCurrentTurn(fullGameData.Turn, fullGameData.Round);
            SetupVisualUpdates();
            fullGameData.PlayersData.ForEach(p =>
            {
                var player = PlayerManager.GetPlayerById(p.Player.Id);
                if (player == null) return;

                player.AddArtifactsTaken(p.Artifacts.ArtifactsOwned);
                player.PlayerArtifactManager.AddArtifactsPlayed(p.Artifacts.ArtifactsPlayed);
                player.PlayerRoyalCardManager.SetRoyalData(p.RoyalCardsData);
                player.PlayerMercenariesManager.SetAllMercenaries(p.Mercenaries, player);
                player.ResourceManager.GoldIncome = p.GoldIncome;
                player.ResourceManager.SetupResources(p.Resources);
                player.ResourceHeroManager.SetupResources(p.ResourceHero);
                player.PlayerHeroCardManager.SetAllDate(p.Heroes);
                player.TokenManager.AddTokens(p.Tokens);
                player.Morale = p.Morale;
                player.PlayerAuraManager.SetupAuras(p.Auras);
                player.AlreadyPlayedCurrentPhase = p.AlreadyPlayedTurn;
                player.SetBoolStorage(p.BoolStorage);
            });

            PhaseManager.SetCurrentPhase(new Phase {Name = fullGameData.CurrentPhase});
            if(fullGameData.CurrentMiniPhase != null){
                MiniPhaseManager.SetCurrentPhase(new MiniPhase {Name = fullGameData.CurrentMiniPhase.Value});
            }
            

            GameVisualManager.GameTiles.AddTokensToTiles(fullGameData.TokenSetup);
            PlayerManager.SetupMoralesPlayers(fullGameData.PlayerBasedOnMorales);
            
            AdditionalSettingsBasedOnMiniPhase();
        }

        public void AdditionalSettingsBasedOnMiniPhase(){
            if(MiniPhaseManager.CurrentPhase?.Name == MiniPhaseType.RotatePawnMiniPhase){
                GameVisualManager.TilesBorderManager.SetRotateConnections();
            }
        }

        public void ConnectPlayer(bool connected, PlayerViewModel player)
        {
            var _player = PlayerManager.GetPlayerById(player.Id);
            if (_player == null) return;

            _player.IsConnected = connected;

        }

        public void SwapTokens(SwapTokensDataEventData data){
            GameVisualManager.GameTiles.SwapTokens(data.TileOneId, data.TileTwoId);
            TokenManager.ClearSelection();
        }

        public void AddResource(ResourceReceivedEventData data)
        {
            var player = PlayerManager.GetPlayerById(data.PlayerId);
            if (player == null) return;

            player.ResourceManager.AddResources(data.Resources);
        }

        public void BanishRoyalCar(BanishRoyalCardEventData data)
        {
            RolayCardsManager.BanishRoyalCard(data.RoyalCard.Id);
        }

        public void BuffHero(BuffHeroData buffHeroData)
        {
            var player = PlayerManager.GetPlayerById(buffHeroData.PlayerId);
            if (player == null) return;

            var heroCard = player.PlayerHeroCardManager.GetHeroCardById(buffHeroData.HeroId);

            if (heroCard == null) return;

            foreach (var resource in buffHeroData.HeroResourcesNew)
            {

                ResourceHeroType resourceType = resource.Key;
                int resourceValue = resource.Value;

                player.ResourceHeroManager.AddResource(resourceType, resourceValue / 2);

                switch (resourceType)
                {
                    case ResourceHeroType.Army:
                        heroCard.Army = resourceValue;
                        break;
                    case ResourceHeroType.Magic:
                        heroCard.Magic = resourceValue;
                        break;
                    case ResourceHeroType.Siege:
                        heroCard.Siege = resourceValue;
                        break;
                    default:

                        break;
                }
            }
        }
    }
}