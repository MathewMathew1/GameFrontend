

using System;
using System.ComponentModel;

namespace BoardGameFrontend.Models
{
    public enum MiniPhaseType
    {
        TeleportationPhase,
        MercenaryRerollPhase,
        ArtifactPickPhase,
        DummyPhase,
        FulfilProphecyPhase,
        LockMercenaryPhase,
        BuffHeroPhase,
        BlockTilePhase,
        RoyalCardPickMiniPhase,
        ReplayArtifactMiniPhase,
        ReplaceHeroMiniPhase,
        BanishRoyalCard,
        SwapTokens,
        RotatePawnMiniPhase,
        ReplaceHeroToBuyMiniPhase
    }

    public static class MiniPhaseTypeExtensions
    {
        public static string GetFriendlyName(this MiniPhaseType phaseType)
        {
            return phaseType switch
            {
                MiniPhaseType.TeleportationPhase => "Teleport Phase",
                MiniPhaseType.MercenaryRerollPhase => "Mercenary Reroll",
                MiniPhaseType.ArtifactPickPhase => "Pick Artifact Mini Phase",
                MiniPhaseType.DummyPhase => "Dummy Phase",
                MiniPhaseType.FulfilProphecyPhase => "Fulfill Prophecy Phase",
                MiniPhaseType.LockMercenaryPhase => "Lock Mercenary Phase",
                MiniPhaseType.BuffHeroPhase => "Buff Hero Phase",
                MiniPhaseType.BlockTilePhase => "Move Block Token Phase",
                MiniPhaseType.RoyalCardPickMiniPhase => "Royal Card Pick Phase",
                MiniPhaseType.ReplayArtifactMiniPhase => "Replay Artifact Phase",
                MiniPhaseType.BanishRoyalCard => "Banish Royal Card Phase",
                MiniPhaseType.SwapTokens => "Swap Tokens Phase",
                MiniPhaseType.RotatePawnMiniPhase => "Rotate Pawn Phase",
                MiniPhaseType.ReplaceHeroToBuyMiniPhase => "Replace Hero to buy",
                _ => phaseType.ToString() 
            };
        }
    }

    public class MiniPhase : INotifyPropertyChanged
    {
        private MiniPhaseType _name;
        public MiniPhaseType Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string FriendlyName => Name.GetFriendlyName();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MiniPhaseDataWithDifferentPlayer{
        public required Guid PlayerId {get; set;}
    }


}