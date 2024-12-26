using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BoardGameFrontend.Models
{
    public class IconAtlas
    {
        public required int Id { get; set; }
        public required string NameType { get; set; }
        public required int IconWidth { get; set; }
        public required int IconHeight { get; set; }
        public required int Columns { get; set; }
        public required string ImagePath { get; set; }

        public IconStruct GetImageInfo(int index){
            int iX = (index % Columns) * IconWidth;
            int iY = (index / Columns) * IconHeight;  
            return new IconStruct {OffsetX=iX, OffsetY=iY, ImagePath=ImagePath, IconHeight = IconHeight, IconWidth = IconWidth};     
        }

        public CroppedBitmap GetImageSourceByIndex(int index)
        {
            BitmapImage biptest = new BitmapImage(new Uri(ImagePath));
            int iX = (index % Columns) * IconWidth;
            int iY = (index / Columns) * IconHeight;  
            var cropRect = new Int32Rect(iX, iY, IconWidth, IconHeight);
            return new CroppedBitmap(biptest, cropRect);
        }
    }
    
    public class IconStruct
    {
        public required int IconHeight { get; set;}
        public required int IconWidth{ get; set;}
        public required int OffsetX { get; set;}
        public required int OffsetY { get; set;}
        public required string ImagePath { get; set;}
    }
}