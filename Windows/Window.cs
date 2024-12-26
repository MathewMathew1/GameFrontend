using System.Windows;

namespace BoardGameFrontend.Windows
{
    public class FullScreenWindow : Window
    {
        public FullScreenWindow()
        {
            this.WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.CanResize;
              this.Top = 0;
            this.Left = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
        }
    }
}