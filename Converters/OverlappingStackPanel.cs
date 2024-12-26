using System.Windows;
using System.Windows.Controls;

namespace BoardGameFrontend.Windows
{
    public class OverlappingStackPanel : StackPanel
    {
        public static readonly DependencyProperty MaxOverlapProperty =
            DependencyProperty.Register(nameof(MaxOverlap), typeof(double), typeof(OverlappingStackPanel), new PropertyMetadata(20.0));

        public static readonly DependencyProperty ReverseOrderProperty =
            DependencyProperty.Register(nameof(ReverseOrder), typeof(bool), typeof(OverlappingStackPanel), new PropertyMetadata(false));

        public double MaxOverlap
        {
            get => (double)GetValue(MaxOverlapProperty);
            set => SetValue(MaxOverlapProperty, value);
        }

        public bool ReverseOrder
        {
            get => (bool)GetValue(ReverseOrderProperty);
            set => SetValue(ReverseOrderProperty, value);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double offset = 0;
            double overlap = MaxOverlap;

            if (InternalChildren.Count > 1)
            {
                overlap = MaxOverlap * (InternalChildren.Count - 1) / InternalChildren.Count;
            }

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var child = InternalChildren[i];
                var childDesiredSize = child.DesiredSize;

                if (ReverseOrder)
                {
                    child.Arrange(new Rect(offset, 0, childDesiredSize.Width, finalSize.Height));
                }
                else
                {
                    child.Arrange(new Rect(finalSize.Width - childDesiredSize.Width - offset, 0, childDesiredSize.Width, finalSize.Height));
                }

                offset += childDesiredSize.Width - overlap;
            }

            return finalSize;
        }
    }
}
