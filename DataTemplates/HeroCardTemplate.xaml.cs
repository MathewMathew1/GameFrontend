using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Windows
{
    public partial class HeroCardTemplate : UserControl
    {
        public HeroCardTemplate()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MercenaryProperty =
        DependencyProperty.Register("Hero", typeof(HeroCardDisplay), typeof(HeroCardTemplate), new PropertyMetadata(null));

        public HeroCardDisplay Hero
        {
            get => (HeroCardDisplay)GetValue(MercenaryProperty);
            set => SetValue(MercenaryProperty, value);
        }

        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register("SelectCommand", typeof(ICommand), typeof(HeroCardTemplate), new PropertyMetadata(null));

        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

        public static readonly DependencyProperty IsYourCardProperty =
            DependencyProperty.Register("IsYourCard", typeof(bool), typeof(HeroCardTemplate), new PropertyMetadata(false));

        public bool IsYourCard
        {
            get { return (bool)GetValue(IsYourCardProperty); }
            set { SetValue(IsYourCardProperty, value); }
        }

        public static readonly DependencyProperty LeftToYourRightProperty =
            DependencyProperty.Register("LeftToRight", typeof(bool), typeof(HeroCardTemplate), new PropertyMetadata(false));

        public bool LeftToRight
        {
            get { return (bool)GetValue(LeftToYourRightProperty); }
            set { SetValue(LeftToYourRightProperty, value); }
        }

        public static readonly DependencyProperty ShowDescriptionTooltipProperty =
            DependencyProperty.Register("ShowDescriptionTooltip", typeof(bool), typeof(HeroCardTemplate), new PropertyMetadata(false));

        public bool ShowDescriptionTooltip
        {
            get { return (bool)GetValue(ShowDescriptionTooltipProperty); }
            set { SetValue(ShowDescriptionTooltipProperty, value); }
        }
    }
}