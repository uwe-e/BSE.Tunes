using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Controls.Extensions
{
    /// <summary>
    /// Extension methods and attached properties for the Flyout class.
    /// 
    /// source http://marcominerva.wordpress.com/2013/07/30/using-windows-8-1-flyout-xaml-control-with-mvvm/
    /// 
    /// </summary>
    public static class FlyoutExtensions
    {
        /// <summary>
        /// Gets the identifier for the IsOpen dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.RegisterAttached("IsOpen",
            typeof(bool),
            typeof(FlyoutExtensions), new PropertyMetadata(false, OnIsOpenChanged));
        /// <summary>
        /// Gets the identifier for the Parent dependency property. 
        /// </summary>
        public static readonly DependencyProperty ParentProperty =
            DependencyProperty.RegisterAttached("Parent", typeof(FrameworkElement),
            typeof(FlyoutExtensions), new PropertyMetadata(null, OnParentChanged));
        public static readonly DependencyProperty OffsetPointProperty =
            DependencyProperty.RegisterAttached("OffsetPoint", typeof(Point),
                typeof(FlyoutExtensions), new PropertyMetadata(null));
        /// <summary>
        /// Set the IsOpen value.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="value">true, if the flyout should be shown, otherwise false.</param>
        public static void SetIsOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOpenProperty, value);
        }
        /// <summary>
        /// Get the IsOpen value.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <returns>true, if the flyout should be shown, otherwise false.</returns>
        public static bool GetIsOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOpenProperty);
        }
        /// <summary>
        /// Sets the flyout´s parent.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="value">The parent of the flyout.</param>
        public static void SetParent(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(ParentProperty, value);
        }
        /// <summary>
        /// Gets the flyout´s parent.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <returns>he parent of the flyout.</returns>
        public static FrameworkElement GetParent(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(ParentProperty);
        }
        public static void SetOffsetPoint(DependencyObject obj, Point value)
        {
            obj.SetValue(OffsetPointProperty, value);
        }
        public static Point GetOffsetPoint(DependencyObject obj)
        {
            return (Point)obj.GetValue(OffsetPointProperty);
        }
        /// <summary>
        /// Handles changes to the Parent property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnParentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var flyout = obj as Flyout;
            if (flyout != null)
            {
                flyout.Opening += (s, args) =>
                {
                    flyout.SetValue(IsOpenProperty, true);
                };

                flyout.Closed += (s, args) =>
                {
                    flyout.SetValue(IsOpenProperty, false);
                };
            }
        }
        /// <summary>
        /// Handles changes to the IsOpen property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnIsOpenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var flyout = obj as MenuFlyout;
            var parent = (FrameworkElement)obj.GetValue(ParentProperty);
            var offsetPoint = (Point)obj.GetValue(OffsetPointProperty);
            //var fe = sender as FrameworkElement;
            //var menu = (MenuFlyout)FlyoutBase.GetAttachedFlyout(fe);
            //menu.ShowAt(fe, e.GetPosition(fe));
            
            if (flyout != null && parent != null)
            {
                // trycatch prevents the
                // WinRT information: Placement target needs to be in the visual tree.
                // exception.
                try
                {
                    var newValue = (bool)e.NewValue;
                    if (newValue)
                    {
                        flyout.Closed += OnFlyoutClosed;
                        flyout.ShowAt(parent, offsetPoint);
                    }
                    else
                    {
                        flyout.Hide();
                    }
                }
                catch { }
            }
        }

        private static void OnFlyoutClosed(object sender, object e)
        {
            SetIsOpen(sender as DependencyObject, false);
        }
    }
}
