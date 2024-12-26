
namespace BoardGameFrontend.Models
{
    public class BorderVisual
    {
        public string ImagePath { get; }

        private static readonly string[] ImagePaths =
        {
        "pack://application:,,,/Assets/Borders/BorderVisualsFaction1.png",
        "pack://application:,,,/Assets/Borders/BorderVisualsFaction2.png",
        "pack://application:,,,/Assets/Borders/BorderVisualsFaction3.png",
        "pack://application:,,,/Assets/Borders/BorderVisualsFaction4.png"
    };

        public BorderVisual(int factionNumber)
        {
            if (factionNumber < 1 || factionNumber > ImagePaths.Length)
            {
                factionNumber = 1;
            }

            ImagePath = ImagePaths[factionNumber - 1];
        }
    }
}