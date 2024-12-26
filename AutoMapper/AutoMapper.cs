using AutoMapper;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.AutoMapper
{
    public static class AutoMapperConfig
    {
        static AutoMapperConfig()
        {
            Initialize();
        }

        public static IMapper Mapper { get; private set; } = default!;

        public static void Initialize()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
                cfg.ConstructServicesUsing(type => new TileTypeResolver(TileTypes.Tiles));
            });

            Mapper = configuration.CreateMapper();
        }
    }
}