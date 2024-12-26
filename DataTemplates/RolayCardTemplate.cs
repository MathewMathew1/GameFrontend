using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Windows
{
    public partial class RolayCardTemplate : UserControl
    {
        public RolayCardTemplate()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty RolayCardProperty =
        DependencyProperty.Register("RolayCard", typeof(RolayCardDisplay), typeof(RolayCardTemplate), new PropertyMetadata(null));

        public RolayCardDisplay RolayCard
        {
            get => (RolayCardDisplay)GetValue(RolayCardProperty);
            set => SetValue(RolayCardProperty, value);
        }

        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register("SelectCommand", typeof(ICommand), typeof(RolayCardTemplate), new PropertyMetadata(null));

        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

         public static readonly DependencyProperty IsYourCardProperty =
            DependencyProperty.Register("IsYourCard", typeof(bool), typeof(RolayCardTemplate), new PropertyMetadata(true));

        public bool IsYourCard
        {
            get { return (bool)GetValue(IsYourCardProperty); }
            set { SetValue(IsYourCardProperty, value); }
        }

    }
}