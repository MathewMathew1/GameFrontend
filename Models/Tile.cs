using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using AutoMapper;
using BoardGameFrontend.VisualManager;

namespace BoardGameFrontend.Models
{
    public class Connection
    {
        public int ToId { get; set; }
        public required List<int> Reqs { get; set; }
    }

    public class TokenReward
    {
        public required Reward Reward { get; set; }
    }

    public class Tile : INotifyPropertyChanged
    {
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int Id { get; set; }
        public int RotateID {get; set;}
        public int TileTypeId { get; set; }
        public List<List<int>> CastleRange { get; set; }
        private Token? _token;
        public Token? Token
        {
            get => _token;
            set
            {
                if (_token != value)
                {
                    _token = value;
                    OnPropertyChanged(nameof(Token));
                }
            }
        }

        public required List<Connection> Connections { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Resource
    {
        public ResourceType Type { get; set; }
        public int Amount { get; set; }

        public Resource(ResourceType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }

    public class TileReward
    {
        public List<Resource> Resources { get; set; }
        public int? ExperiencePoints { get; set; }
        public int? TeleportedTileId { get; set; }
        public bool? RerollMercenaryAction { get; set; }
        public bool? GetRandomArtifact { get; set; }
        public Artifact? Artifact { get; set; }
        public bool GotArtifact { get; set; }
        public TokenReward? TokenReward { get; set; }
        public bool EmptyMovement { get; set; } = false;
        public bool TempSignet { get; set; } = false;


        public TileReward()
        {
            Resources = new List<Resource>();
        }
    }

    public class MoveOnTileData
    {
        public required TileReward TileReward { get; set; }
        public required PlayerViewModel Player { get; set; }
        public int MovementFullLeft { get; set; }
        public int MovementUnFullLeft { get; set; }
        public int TileId { get; set; }
        public bool AdjacentMovement { get; set; }
    }

    public class SwapTokensDataEventData
    {
        public required int TileOneId { get; set; }
        public required int TileTwoId { get; set; }
        public required Guid PlayerId {get; set;}
    }

    public class TileRewardData
    {
        public required TileReward TileReward { get; set; }
        public required PlayerViewModel Player { get; set; }
    }

    public class MoveOnTileOnEvent
    {
        public required AurasType? AuraUsed { get; set; }
        public required PlayerViewModel Player { get; set; }
        public int TileId { get; set; }
    }

    public class TileWithType : INotifyPropertyChanged
    {
        private int _offsetX;
        public int OffsetX
        {
            get => _offsetX;
            set
            {
                if (_offsetX != value)
                {
                    _offsetX = value;
                    OnPropertyChanged(nameof(OffsetX));
                }
            }
        }

        private int _offsetY;
        public int OffsetY
        {
            get => _offsetY;
            set
            {
                if (_offsetY != value)
                {
                    _offsetY = value;
                    OnPropertyChanged(nameof(OffsetY));
                }
            }
        }
        public int Id { get; set; }
        public required TileType TileType { get; set; }
        public required List<List<int>> CastleRange { get; set; }
        public int RotateID {get; set;}
        private Token? _token;
        public Token? Token
        {
            get => _token;
            set
            {
                if (_token != value)
                {
                    _token = value;
                    OnPropertyChanged(nameof(Token));
                }
            }
        }
        public required List<Connection> Connections { get; set; }

        public bool IsInRangeOfCastle(Fraction fraction, int distance = 3)
        {
            var isInRange = false;
            CastleRange.ForEach(castle =>
            {
                if (castle[0] == fraction.Id && castle[1] <= distance)
                {
                    isInRange = true;
                    return;
                }
            });

            return isInRange;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TileType
    {
        public required int Id { get; set; }
        public int? Req {get; set;}
        public required string Name { get; set; }
        public required int ImageIndex { get; set; }
        public required int OffsetX { get; set; }
        public required int OffsetY { get; set; }
        public required string IconAtlas { get; set; }
        public string ImagePathString { get; set; } = "";

        public TileType()
        {
        }

        public void SetupTile()
        {
            if (!string.IsNullOrEmpty(IconAtlas))
            {
                var iconStruct = IconAtlases.GetIconAtlasByNameType(IconAtlas).GetImageInfo(ImageIndex);

                OffsetX = iconStruct.OffsetX;
                OffsetY = iconStruct.OffsetY;
                ImagePathString = iconStruct.ImagePath;
            }
        }

        public Int32Rect CropRect => new Int32Rect(
            OffsetX,
            OffsetY,
            256,
            256
        );

    }

    public static class TileTypes
    {

        public static readonly List<TileType> Tiles;

        static TileTypes()
        {
            string filePath = "Data/TileTypes.json";
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                Tiles = JsonSerializer.Deserialize<List<TileType>>(jsonData) ?? new List<TileType>();
                Tiles.ForEach(tile => tile.SetupTile());
            }
            else
            {
                Tiles = new List<TileType>();
            }
        }
    }

    public class TileTypeResolver : IValueResolver<Tile, TileWithType, TileType>
    {
        private readonly IEnumerable<TileType> _tileTypes;

        public TileTypeResolver(IEnumerable<TileType> tileTypes)
        {
            _tileTypes = tileTypes;
        }

        public TileType Resolve(Tile source, TileWithType destination, TileType destMember, ResolutionContext context)
        {
            return _tileTypes.FirstOrDefault(t => t.Id == source.TileTypeId);
        }
    }

    public class TeleportationData
    {
        public required PlayerViewModel Player { get; set; }
        public int TileId { get; set; }
    }

    public class BlockedTileData
    {
        public required Guid PlayerId { get; set; }
        public required int TileId { get; set; }
        public required Token Token { get; set; }
    }

    public class GoldIntoMovementEventData
    {
        public required int MovementFullLeft { get; set; }
        public required Guid PlayerId { get; set; }
        public required int GoldLeft {get; set;}
    }

    public class FullMovementIntoEmptyEventData
    {
        public required int MovementFullLeft { get; set; }
        public required int MovementUnFullLeft { get; set; }
        public required Guid PlayerId { get; set; }
    }

    public class RotateTileEventData{
        public required int TileId {get; set;}
        public required Guid PlayerId {get; set;}
    }
}