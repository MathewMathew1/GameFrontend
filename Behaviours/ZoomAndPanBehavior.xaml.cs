using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BoardGameFrontend.Behaviors
{
    public static class ZoomAndPanBehavior
    {
        private static Point _start;
        private static Point _origin;
        private static bool _isDragging;

        public static readonly DependencyProperty EnableZoomAndPanProperty =
            DependencyProperty.RegisterAttached(
                "EnableZoomAndPan",
                typeof(bool),
                typeof(ZoomAndPanBehavior),
                new PropertyMetadata(false, OnEnableZoomAndPanChanged));

        public static bool GetEnableZoomAndPan(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableZoomAndPanProperty);
        }

        public static void SetEnableZoomAndPan(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableZoomAndPanProperty, value);
        }

        private static void OnEnableZoomAndPanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                if ((bool)e.NewValue)
                {
                    scrollViewer.PreviewMouseWheel += OnMouseWheelZoom;
                    scrollViewer.PreviewMouseDown += OnMouseDown;
                    scrollViewer.PreviewMouseMove += OnMouseMove;
                    scrollViewer.MouseUp += OnMouseUp;
                }
                else
                {
                    scrollViewer.PreviewMouseWheel -= OnMouseWheelZoom;
                    scrollViewer.PreviewMouseDown -= OnMouseDown;
                    scrollViewer.PreviewMouseMove -= OnMouseMove;
                    scrollViewer.MouseUp -= OnMouseUp;
                }
            }
        }

        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer && e.LeftButton == MouseButtonState.Pressed)
            {
                var source = e.Source as DependencyObject;

                if ((source == null || !(source is Button)) && !IsParentButton(source))
                {
                    var grid = scrollViewer.Content as Grid;
                    _start = e.GetPosition(grid);
                    _origin = new Point(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
                    _isDragging = true; // Initialize dragging state
                }
            }
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer && e.LeftButton == MouseButtonState.Pressed && _isDragging)
            {
                 var source = e.Source as DependencyObject;
                var grid = scrollViewer.Content as Grid;
                if (grid != null && !IsParentButton(source))
                {
                    Point currentPosition = e.GetPosition(grid);
                    Vector delta = _start - currentPosition;

                    // If the mouse has moved more than a small threshold, consider it a drag
                    if (Math.Abs(delta.X) > 5 || Math.Abs(delta.Y) > 5)
                    {
                        _isDragging = true; // Now it is considered a drag

                        var transformGroup = grid.RenderTransform as TransformGroup;
                        if (transformGroup != null)
                        {
                            var translateTransform = transformGroup.Children.Count > 1 ? (TranslateTransform)transformGroup.Children[1] : new TranslateTransform();
                            double panSensitivity = 0.5;

                            translateTransform.X -= delta.X * panSensitivity;
                            translateTransform.Y -= delta.Y * panSensitivity;

                            ConstrainPanning(scrollViewer, grid);
                        }

                        // Capture the mouse if it's dragging
                        scrollViewer.CaptureMouse();
                    }
                }
            }
        }

        private static void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            if (sender is ScrollViewer scrollViewer)
            {
                if (!_isDragging)
                {
                    // It's a click, do not capture the mouse
                    scrollViewer.ReleaseMouseCapture();
                }

                 // Reset dragging state after mouse is released
            }
        }

        private static void OnMouseWheelZoom(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer && scrollViewer.Content is Grid grid)
            {
                var transformGroup = grid.RenderTransform as TransformGroup;
                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    grid.RenderTransform = transformGroup;
                }

                if (transformGroup.Children.Count == 0)
                    transformGroup.Children.Add(new ScaleTransform());

                if (transformGroup.Children.Count == 1)
                    transformGroup.Children.Add(new TranslateTransform());

                var scaleTransform = (ScaleTransform)transformGroup.Children[0];

                double zoomFactor = e.Delta > 0 ? 0.1 : -0.1;
                double newScale = Math.Max(1.0, scaleTransform.ScaleX + zoomFactor);

                scaleTransform.ScaleX = newScale;
                scaleTransform.ScaleY = newScale;

                ConstrainPanning(scrollViewer, grid);

                e.Handled = true;
            }
        }

        private static void ConstrainPanning(ScrollViewer scrollViewer, Grid grid)
        {
            if (grid.RenderTransform is TransformGroup transformGroup)
            {
                var scaleTransform = transformGroup.Children.Count > 0 ? (ScaleTransform)transformGroup.Children[0] : new ScaleTransform();
                var translateTransform = transformGroup.Children.Count > 1 ? (TranslateTransform)transformGroup.Children[1] : new TranslateTransform();

                if (scaleTransform == null || translateTransform == null)
                    return;

                double scaleX = scaleTransform.ScaleX;
                double scaleY = scaleTransform.ScaleY;

                double scaledWidth = grid.ActualWidth * scaleX;
                double scaledHeight = grid.ActualHeight * scaleY;

                double viewportWidth = scrollViewer.ViewportWidth;
                double viewportHeight = scrollViewer.ViewportHeight;

                double minTranslateX = Math.Min(0, viewportWidth - scaledWidth);
                double minTranslateY = Math.Min(0, viewportHeight - scaledHeight);

                double maxTranslateX = 0;
                double maxTranslateY = 0;

                translateTransform.X = Math.Max(minTranslateX, Math.Min(translateTransform.X, maxTranslateX));
                translateTransform.Y = Math.Max(minTranslateY, Math.Min(translateTransform.Y, maxTranslateY));

                grid.RenderTransform = transformGroup;
            }
        }

        private static bool IsParentButton(DependencyObject element)
        {
            while (element != null)
            {
                if (element is Button)
                {
                    return true;
                }

                element = VisualTreeHelper.GetParent(element);
            }
            return false;
        }
    }

}

