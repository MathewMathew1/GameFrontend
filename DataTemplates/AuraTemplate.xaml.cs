using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Windows
{
    public partial class AuraTemplate : UserControl
    {
        public AuraTemplate()
        {
            InitializeComponent();
        }

        // Define the dependency property for Aura
        public static readonly DependencyProperty AuraProperty =
            DependencyProperty.Register(
                "Aura",
                typeof(AuraTypeWithLongevity),
                typeof(AuraTemplate),
                new PropertyMetadata(null, OnAuraChanged));

        public static readonly DependencyProperty AuraDisplayInfoProperty =
            DependencyProperty.Register(
                "AuraDisplayInfo",
                typeof(AuraExtendedInfoDisplay),
                typeof(AuraTemplate),
                new PropertyMetadata(null));

        // The Aura property to bind the AuraTypeWithLongevity object
        public AuraTypeWithLongevity Aura
        {
            get => (AuraTypeWithLongevity)GetValue(AuraProperty);
            set => SetValue(AuraProperty, value);
        }

        public AuraExtendedInfoDisplay AuraDisplayInfo
        {
            get => (AuraExtendedInfoDisplay)GetValue(AuraDisplayInfoProperty);
            private set => SetValue(AuraDisplayInfoProperty, value);
        }
    
        private static void OnAuraChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AuraTemplate control && e.NewValue is AuraTypeWithLongevity newAura)
            {
                control.UpdateAuraDisplayInfo(newAura);
            }
        }

        private void UpdateAuraDisplayInfo(AuraTypeWithLongevity aura)
        {
            if (aura != null)
            {
                int imageIndex = aura.Value1 != null? aura.Value1.Value: 1; 
                AuraDisplayInfo = AuraFactory.GetAuraDisplayInfo(aura.Aura, imageIndex);
            }
        }


    }
}