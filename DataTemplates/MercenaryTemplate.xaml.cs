using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Windows
{
    public partial class MercenaryTemplate : UserControl
    {
        public MercenaryTemplate()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MercenaryProperty =
        DependencyProperty.Register("Mercenary", typeof(MercenaryDisplay), typeof(MercenaryTemplate), new PropertyMetadata(null));

        public MercenaryDisplay Mercenary
        {
            get => (MercenaryDisplay)GetValue(MercenaryProperty);
            set => SetValue(MercenaryProperty, value);
        }

        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register("SelectCommand", typeof(ICommand), typeof(MercenaryTemplate), new PropertyMetadata(null));

        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

        public static readonly DependencyProperty IsYourCardProperty =
            DependencyProperty.Register("IsYourCard", typeof(bool), typeof(MercenaryTemplate), new PropertyMetadata(false));

        public bool IsYourCard
        {
            get { return (bool)GetValue(IsYourCardProperty); }
            set { SetValue(IsYourCardProperty, value); }
        }
    }
}