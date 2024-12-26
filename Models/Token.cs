using System.Collections.Generic;
using System.Windows;
using BoardGameFrontend.VisualManager;

namespace BoardGameFrontend.Models
{
    public class DisplayInfo{
        public int OffsetY  { get; set; } = 128;
        public int OffsetX  { get; set; } = 128;
        
        public string ImagePathString { get; set; } = "";
        public int IconWidth  { get; set; } = 128;
        public int IconHeight  { get; set; } = 128;

        public Int32Rect CropRect => new Int32Rect(
            OffsetX,
            OffsetY,
            IconWidth , 
            IconHeight
        );


        public void SetupData(string imageAtlas, int imageIndex)
        {
            if (!string.IsNullOrEmpty(imageAtlas))
            {
                var iconStruct = IconAtlases.GetIconAtlasByNameType(imageAtlas).GetImageInfo(imageIndex);
                IconHeight = iconStruct.IconHeight;
                IconWidth = iconStruct.IconWidth;
                OffsetX = iconStruct.OffsetX;
                OffsetY = iconStruct.OffsetY;
                ImagePathString = iconStruct.ImagePath;
            }
        }
    }
          

    public class Token
    {
        public required int Id { get; set; }
        public required string Type { get; set; }
        public required string ImageAtlas { get; set; }
        public required int EffectType { get; set; }
        public required int EffectID { get; set; }
        public required string ToolTipText { get; set; }
        public DisplayInfo DisplayInfo {get; set;} = new DisplayInfo();
        private int _imageIndex;
        public required bool Collectable {get; set;}
        public required bool InStartingPool {get; set;}
        public int ImageIndex
        {
            get => _imageIndex;
            set
            {
                _imageIndex= value;
                DisplayInfo = new DisplayInfo();
                DisplayInfo.SetupData(ImageAtlas, ImageIndex);
            }
        }
    }

    public class TokenTileInfo
    {
        public required Token Token { get; set; }
        public required int TileId { get; set; }
    }

    public class NewTokensSetupEventData{
        public required List<TokenTileInfo> NewTokens {get; set;}
    }
}