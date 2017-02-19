using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

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
        #region IsOpen
        /// <summary>
        /// Gets the identifier for the IsOpen dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.RegisterAttached("IsOpen",
            typeof(bool),
            typeof(FlyoutExtensions), new PropertyMetadata(false, OnIsOpenChanged));
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
        /// Handles changes to the IsOpen property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnIsOpenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //var flyout = obj as MenuFlyout;
            var flyout = obj as FlyoutBase;
            var parent = (FrameworkElement)obj.GetValue(ParentProperty);
            var offsetPoint = (Point)obj.GetValue(OffsetPointProperty);

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
                        if (offsetPoint.Equals(new Point()))
                        {
                            flyout.ShowAt(parent);
                        }
                        else
                        {
                            MenuFlyout menuFlyout = flyout as MenuFlyout;
                            if (menuFlyout != null)
                            {
                                menuFlyout.ShowAt(parent, offsetPoint);
                            }
                            else
                            {
                                flyout.ShowAt(parent);
                            }
                        }

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
        #endregion

        #region Parent
        /// <summary>
        /// Gets the identifier for the Parent dependency property. 
        /// </summary>
        public static readonly DependencyProperty ParentProperty =
            DependencyProperty.RegisterAttached("Parent", typeof(FrameworkElement),
            typeof(FlyoutExtensions), new PropertyMetadata(null, OnParentChanged));
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
        /// <summary>
        /// Handles changes to the Parent property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnParentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var flyout = obj as FlyoutBase;
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
        #endregion

        #region  OffsetPoint
        public static readonly DependencyProperty OffsetPointProperty =
            DependencyProperty.RegisterAttached("OffsetPoint", typeof(Point),
                typeof(FlyoutExtensions), new PropertyMetadata(null));
        public static void SetOffsetPoint(DependencyObject obj, Point value)
        {
            obj.SetValue(OffsetPointProperty, value);
        }
        public static Point GetOffsetPoint(DependencyObject obj)
        {
            return (Point)obj.GetValue(OffsetPointProperty);
        }
        #endregion

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached("ItemsSource", typeof(IEnumerable),
                typeof(FlyoutExtensions), new PropertyMetadata(null, ItemsSourceChanged));

        public static void SetItemsSource(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(ItemsSourceProperty, value);
        }

        public static IEnumerable GetItemsSource(DependencyObject obj)
        {
            return obj.GetValue(ItemsSourceProperty) as IEnumerable;
        }
        public static readonly DependencyProperty ItemTemplateProperty =
          DependencyProperty.RegisterAttached("ItemTemplate", typeof(DataTemplate),
              typeof(FlyoutExtensions), new PropertyMetadata(null, ItemsTemplateChanged));

        public static void SetItemTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(ItemTemplateProperty, value);
        }

        public static DataTemplate GetItemTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(ItemTemplateProperty);
        }

        private static void ItemsTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            InitializeItemsSource(obj as FlyoutBase);
        }
        private static void ItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            InitializeItemsSource(obj as FlyoutBase);
        }
        private static async void InitializeItemsSource(FlyoutBase flyoutBase)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }
            Flyout flyout = flyoutBase as Flyout; 
            var itemsSource = GetItemsSource(flyout);
            if (itemsSource == null)
            {
                return;
            }
            var itemTemplaate = GetItemTemplate(flyout);
            if (itemTemplaate == null)
            {
                return;
            }
            var itemsControl = new Windows.UI.Xaml.Controls.ItemsControl
            {
                ItemsSource = itemsSource,
                ItemTemplate = itemTemplaate,
            };

            Windows.UI.Core.DispatchedHandler dispatchedHandler = () => flyout.Content = itemsControl;
            await flyout.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, dispatchedHandler);
        }
        #endregion
    }
}
