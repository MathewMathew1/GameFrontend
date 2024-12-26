
using System.Windows;
namespace BoardGameFrontend.Windows
{
    public static class TemplateHelpers
    {
        public static readonly DependencyProperty ReverseFlagOrderProperty =
            DependencyProperty.RegisterAttached("ReverseFlagOrder", typeof(bool), typeof(TemplateHelpers), new PropertyMetadata(false));

        public static bool GetReverseFlagOrder(DependencyObject obj)
        {
            return (bool)obj.GetValue(ReverseFlagOrderProperty);
        }

        public static void SetReverseFlagOrder(DependencyObject obj, bool value)
        {
            obj.SetValue(ReverseFlagOrderProperty, value);
        }
    }
}
