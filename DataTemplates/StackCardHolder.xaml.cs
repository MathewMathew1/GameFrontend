using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace BoardGameFrontend.Windows
{
    public partial class StackCardHolder : UserControl
    {
        public StackCardHolder()
        {
            InitializeComponent();
        }

        // Define the Number dependency property
        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(int), typeof(StackCardHolder), new PropertyMetadata(0));

        public int Number
        {
            get { return (int)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        // Define the BackgroundColor dependency property
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(StackCardHolder), new PropertyMetadata(Brushes.Gray));

        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        // Define the TextColor dependency property
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(Brush), typeof(StackCardHolder), new PropertyMetadata(Brushes.Black));

        public Brush TextColor
        {
            get { return (Brush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }
    }
}
