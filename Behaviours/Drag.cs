using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BoardGameFrontend.Behaviors
{
    public static class DragBehavior
    {
        private static bool _isDragging = false;
        private static Point _startPoint;

        public static readonly DependencyProperty IsDragEnabledProperty =
            DependencyProperty.RegisterAttached("IsDragEnabled", typeof(bool), typeof(DragBehavior),
                new PropertyMetadata(false, OnIsDragEnabledChanged));

        // Drop event
        public static readonly RoutedEvent DropEvent =
            EventManager.RegisterRoutedEvent("Drop", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragBehavior));

        public static void AddDropHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement element)
            {
                element.AddHandler(DropEvent, handler);
            }
        }

        public static void RemoveDropHandler(DependencyObject d, RoutedEventHandler handler)
        {
            if (d is UIElement element)
            {
                element.RemoveHandler(DropEvent, handler);
            }
        }

        public static bool GetIsDragEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDragEnabledProperty);
        }

        public static void SetIsDragEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDragEnabledProperty, value);
        }

        private static void OnIsDragEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.AllowDrop = true;
                    element.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                    element.PreviewMouseMove += OnMouseMove;
                    element.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
                }
                else
                {
                    element.AllowDrop = false;
                    element.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                    element.PreviewMouseMove -= OnMouseMove;
                    element.PreviewMouseLeftButtonUp -= OnMouseLeftButtonUp;
                }
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement element)
            {
                if (Application.Current.MainWindow is LobbyWindow window && window.Game != null)
                {
                    var game = window.Game;
                    var playerCanMove = game.PawnManager.PlayerCanMove();
                    if (!playerCanMove)
                    {
                        return;
                    }
                    _isDragging = true;
                    game.PawnManager.ShowVisualInfo();
                    _startPoint = e.GetPosition(element);
                    element.CaptureMouse();

                    var dragData = new DataObject(typeof(Image), element);

                    DragDrop.DoDragDrop(element, dragData, DragDropEffects.Move);

                    _isDragging = false;
                    game.GameVisualManager.TilesBorderManager.RemoveAvailableConnections();

                }
            }
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && sender is UIElement element)
            {
                var canvas = VisualTreeHelper.GetParent(element) as Canvas;
                if (canvas != null)
                {
                    Point currentPosition = e.GetPosition(canvas);
                    double newLeft = currentPosition.X - _startPoint.X;
                    double newTop = currentPosition.Y - _startPoint.Y;

                    newLeft = Math.Max(0, newLeft);
                    newTop = Math.Max(0, newTop);
                    Canvas.SetLeft(element, newLeft);
                    Canvas.SetTop(element, newTop);
                }
            }
        }

        private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                if (sender is UIElement element)
                {
                    element.ReleaseMouseCapture();
                    RaiseDropEvent(element);
                }
            }
        }

        // Method to raise the Drop event
        private static void RaiseDropEvent(UIElement element)
        {
            var args = new RoutedEventArgs(DropEvent, element);
            element.RaiseEvent(args);
        }

        
    }


}
