using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoMapper;
using BoardGameFrontend.Managers;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map PlayerViewModelData to PlayerViewModel

            CreateMap<User, PlayerViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username));
            CreateMap<Player, PlayerInGameViewModel>()
                .ForMember(dest => dest.ResourceManager, opt => opt.MapFrom(src => 
                    new ResourceManager(new Dictionary<ResourceType, int>{})));
            CreateMap<PlayerViewModelData, PlayerInGameViewModel>()
                .ForMember(dest => dest.ResourceManager, opt => opt.MapFrom(src => 
                    new ResourceManager(src.ResourceManager.Resources))); // Assuming ResourceManager constructor that takes Dictionary
            
            // Map ResourceManagerViewModel to ResourceManager
            CreateMap<ResourceManagerViewModel, ResourceManager>()
                .ForMember(dest => dest.Resources, opt => opt.MapFrom(src => src.Resources));
            CreateMap<HeroCardCombined, HeroCardCombinedDisplay>()
            .ForMember(dest => dest.LeftSide, opt => opt.MapFrom(src => src.LeftSide))
            .ForMember(dest => dest.RightSide, opt => opt.MapFrom(src => src.RightSide));

            CreateMap<HeroCard, HeroCardDisplay>();
            CreateMap<RolayCard, RolayCardDisplay>();
            CreateMap<Mercenary, MercenaryDisplay>();
            CreateMap<ReplacedHero, ReplacedHeroDisplay>();
            CreateMap<Artifact, ArtifactDisplay>();
            CreateMap<Tile, TileWithType>()
            .ForMember(dest => dest.TileType, opt => opt.MapFrom<TileTypeResolver>());
            CreateMap<PlayerInGameViewModel, PlayerViewModel>();
             CreateMap<CurrentHeroCardData, CurrentHeroCard>()
            .ForMember(dest => dest.HeroCard, opt => opt.MapFrom(src => src.HeroCard))
            .ForMember(dest => dest.UnUsedHeroCard, opt => opt.MapFrom(src => src.UnUsedHeroCard))
            .ForMember(dest => dest.MovementFullLeft, opt => opt.MapFrom(src => src.MovementFullLeft))
            .ForMember(dest => dest.MovementUnFullLeft, opt => opt.MapFrom(src => src.MovementUnFullLeft))
            .ForMember(dest => dest.NoFractionMovement, opt => opt.MapFrom(src => src.NoFractionMovement))
            .ForMember(dest => dest.VisitedPlaces, opt => opt.MapFrom(src => new ObservableCollection<int>(src.VisitedPlaces)));
        }
    }
}