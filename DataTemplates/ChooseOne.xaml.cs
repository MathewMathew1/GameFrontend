using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BoardGameFrontend.Windows
{
    public partial class ChooseOne : UserControl, INotifyPropertyChanged
    {
        // Dependency properties for the ImagePath, CropRect, and CommandFunction
        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(ChooseOne), new PropertyMetadata(null));

        public static readonly DependencyProperty CropRectProperty =
            DependencyProperty.Register("CropRect", typeof(Int32Rect), typeof(ChooseOne), new PropertyMetadata(new Int32Rect(0, 0, 100, 100)));

        public static readonly DependencyProperty CommandFunctionProperty =
            DependencyProperty.Register("CommandFunction", typeof(ICommand), typeof(ChooseOne), new PropertyMetadata(null));

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(ChooseOne), new PropertyMetadata(null));

        private bool _firstEffect;
        public bool FirstEffect
        {
            get { return _firstEffect; }
            set
            {
                if (_firstEffect != value)
                {
                    _firstEffect = value;
                    OnPropertyChanged(nameof(FirstEffect));
                }
            }
        }

        // Property for ImagePath (binding the image source)
        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        // Property for CropRect (binding the cropping rectangle)
        public Int32Rect CropRect
        {
            get => (Int32Rect)GetValue(CropRectProperty);
            set => SetValue(CropRectProperty, value);
        }

        // Property for the CommandFunction (Accept button)
        public ICommand CommandFunction
        {
            get => (ICommand)GetValue(CommandFunctionProperty);
            set => SetValue(CommandFunctionProperty, value);
        }

        // Property for the CancelCommand (Cancel button)
        public ICommand CancelCommand
        {
            get => (ICommand)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }

        public ChooseOne()
        {
            InitializeComponent();
        }

        private void TopButton_Click(object sender, RoutedEventArgs e)
        {
            FirstEffect = true; 
        }

        // Click event handler for the Bottom button
        private void BottomButton_Click(object sender, RoutedEventArgs e)
        {
            FirstEffect = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
