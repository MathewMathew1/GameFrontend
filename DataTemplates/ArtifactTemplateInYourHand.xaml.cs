using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Windows
{
    public partial class ArtifactTemplateInYourHand : UserControl
    {
        public ArtifactTemplateInYourHand()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ArtifactProperty =
        DependencyProperty.Register("Artifact", typeof(ArtifactDisplay), typeof(ArtifactTemplateInYourHand), new PropertyMetadata(null));

        public ArtifactDisplay Artifact
        {
            get => (ArtifactDisplay)GetValue(ArtifactProperty);
            set => SetValue(ArtifactProperty, value);
        }

        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register("SelectCommand", typeof(ICommand), typeof(ArtifactTemplateInYourHand), new PropertyMetadata(null));

        public ICommand SelectCommand
        {
            get { return (ICommand)GetValue(SelectCommandProperty); }
            set { SetValue(SelectCommandProperty, value); }
        }

        public static readonly DependencyProperty AlreadyPlayedProperty =
            DependencyProperty.Register("AlreadyPlayed ", typeof(bool), typeof(ArtifactTemplateInYourHand), new PropertyMetadata(false));

        public bool AlreadyPlayed
        {
            get { return (bool)GetValue(AlreadyPlayedProperty ); }
            set { SetValue(AlreadyPlayedProperty , value); }
        }


    }
}