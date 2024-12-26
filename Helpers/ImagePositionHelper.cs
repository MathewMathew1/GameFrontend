using System.Windows;

namespace BoardGameFrontend.Helpers
{
    public static class ImagePositionHelper
    {
        // Constants for image width and height
        public const double ImageWidth = 64.0;
        public const double ImageHeight = 64.0;

        // Method to shift the coordinates by half the image size
        public static Point AdjustPosition(double x, double y)
        {
            int adjustedX = (int)(x - (ImageWidth / 2));
            int adjustedY = (int)(y - (ImageHeight / 2));

            return new Point(adjustedX, adjustedY);
        }
    }
}