using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections.ObjectModel;
using BoardGameFrontend.Models;
using System;
using BoardGameFrontend.AutoMapper;
using System.Linq;
using BoardGameFrontend.Helpers;

namespace BoardGameFrontend.VisualManager
{


    public class GameTiles : INotifyPropertyChanged
    {
    
        public ObservableCollection<TileWithType> Tiles { get; set; } = new ObservableCollection<TileWithType>();
        

        public GameTiles()
        {
            var TilesList = LoadTileMapFromFile("Data/TilesData.json");
            Tiles.Clear(); 
            foreach (var tile in TilesList)
            {
                Tiles.Add(tile); 
            }
            OnPropertyChanged(nameof(Tiles));
        }

        public TileWithType GetTileById(int id){
            return Tiles.FirstOrDefault(tile => tile.Id == id)!;
        }

        public void AddTokensToTiles(List<TokenTileInfo> tokenTileInfos){
            tokenTileInfos.ForEach(info => {
                var tile = GetTileById(info.TileId);
                tile.Token = info.Token;
            });
        }

        public Token? RemoveTokenFromTile(int tileId){
            Token? removedToken = null;
            foreach (var tile in Tiles)
            {
                if(tileId == tile.Id && tile.Token?.Collectable == true){
                    removedToken = tile.Token;
                    tile.Token = null;
                }
            }

            return removedToken;
        }

        public void BlockTile(int tileId, Token token){
            foreach (var tile in Tiles)
            {
                if(tile.Token?.Id == TileHelper.BlockTileTokenId){
                    tile.Token = null;
                }

                if(tileId == tile.Id){
                    tile.Token = token;
                }
            }
        }

        public void SwapTokens(int tileIdOne, int tileIdTwo){
            var tile = GetTileById(tileIdOne);
            var tileTwo = GetTileById(tileIdTwo);

            if(tile == null || tileTwo == null) return;

            if(tile.Token == null || tile.TileType.Id == TileHelper.CastleTileId || tileTwo.Token == null || tileTwo.TileType.Id == TileHelper.CastleTileId) return;

            (tile.Token, tileTwo.Token) = (tileTwo.Token, tile.Token);
        }

        public List<TileWithType> LoadTileMapFromFile(string filePath)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"The file '{filePath}' was not found at path: {fullPath}");
            }

            var jsonData = File.ReadAllText(fullPath);
            var tileList = JsonConvert.DeserializeObject<List<Tile>>(jsonData);
            var tilesWithTypeList = tileList!.Select(data => AutoMapperConfig.Mapper.Map<TileWithType>(data)).ToList();

            return tilesWithTypeList;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}