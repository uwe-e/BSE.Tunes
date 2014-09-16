using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace BSE.Tunes.StoreApp.Controls
{
    public class ContextMenu : ListViewBase
    {
        #region FieldsPrivate
        private Popup m_contextMenuPopup;
        #endregion

        #region DependencyProperties
        /// <summary>
        /// Identifies the Placement dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(Placement), typeof(ContextMenu), new PropertyMetadata(Placement.Default));

        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register("PlacementTarget", typeof(FrameworkElement), typeof(ContextMenu), null);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the orientation of the Popup control when the control opens, and specifies the behavior of the Popup control when it overlaps screen boundaries.
        /// </summary>
        public Placement Placement
        {
            get { return (Placement)this.GetValue(PlacementProperty); }
            set { this.SetValue(PlacementProperty, value); }
        }
        public FrameworkElement PlacementTarget
        {
            get { return (FrameworkElement)this.GetValue(PlacementTargetProperty); }
            set { this.SetValue(PlacementTargetProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="IsOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(Boolean), typeof(ContextMenu), new PropertyMetadata(false, OnIsOpenChanged));
        /// <summary>
        /// An implementation of <see cref="Boolean"/> designed to be used as is open.
        /// </summary>
        public Boolean IsOpen
        {
            get { return (Boolean)this.GetValue(IsOpenProperty); }
            set { this.SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ContextMenu), new PropertyMetadata(0.0));

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ContextMenu), new PropertyMetadata(0.0));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        #endregion

        #region MethodsPublic

        public ContextMenu()
        {
            this.DefaultStyleKey = typeof(ContextMenu);
            this.SelectionMode = ListViewSelectionMode.None;
            this.IsSwipeEnabled = false;
            this.IsItemClickEnabled = true;

            Window.Current.Activated += OnCurrentWindowActivated;
            Window.Current.SizeChanged += OnCurrentWindowSizeChanged;

            this.m_contextMenuPopup = new Popup();
            this.m_contextMenuPopup.Opacity = 0;
            this.m_contextMenuPopup.Opened += OnContextMenuOpened;
            this.m_contextMenuPopup.Closed += OnContextMenuClosed;
            this.m_contextMenuPopup.IsLightDismissEnabled = true;
            this.m_contextMenuPopup.Child = this;
        }
        #endregion

        #region MethodsProtected
        
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }
        
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MenuItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            MenuItem menuItem = element as MenuItem;
            if (menuItem != null && this.ItemTemplate != null)
            {
                menuItem.SetValue(MenuItem.ContentTemplateProperty, this.ItemTemplate);
            }

            SelectorItem selector = element as SelectorItem;
            if (selector != null)
            {
                selector.Tapped += OnSelectorItemTapped;
            }
        }
        
        protected virtual void OnIsOpenChanged(object oldValue, object newValue)
        {
            if ((bool)newValue)
            {
                this.m_contextMenuPopup.IsOpen = (Boolean)newValue;
                this.IsOpen = false;
            }
        }
        
        #endregion

        #region MethodsPrivate
        private void OnCurrentWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.IsOpen = false;
        }

        private void OnCurrentWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            this.IsOpen = false;
        }

        private void OnContextMenuOpened(object sender, object e)
        {
            if (this.m_contextMenuPopup.ActualHeight == 0 || this.m_contextMenuPopup.ActualWidth == 0)
            {
                SizeChangedEventHandler updatePosition = null;
                updatePosition = (s, sizeChangedEventArgs) =>
                {
                    if (sizeChangedEventArgs.NewSize.Width != 0 && sizeChangedEventArgs.NewSize.Height != 0)
                    {
                        OnContextMenuOpened(s, sizeChangedEventArgs);
                        SizeChanged -= updatePosition;
                    }
                };
                SizeChanged += updatePosition;
            }

            this.m_contextMenuPopup.HorizontalOffset = this.HorizontalOffset;
            this.m_contextMenuPopup.VerticalOffset = this.VerticalOffset;

            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            UIElement root = Window.Current.Content;
            if (root != null && this.PlacementTarget != null)
            {
                GeneralTransform transform = this.PlacementTarget.TransformToVisual(root);
                Point relativeSourceLocation = transform.TransformPoint(new Point(0, 0));

                Rect bounds = Window.Current.Bounds;

                double horizondalOffset = 0;
                double verticalOffset = 0;

                horizondalOffset = Math.Min(relativeSourceLocation.X + (this.PlacementTarget.ActualWidth / 2) - (this.PlacementTarget.ActualWidth / 2), bounds.Width - this.m_contextMenuPopup.ActualWidth - 5);
                horizondalOffset = Math.Max(5, horizondalOffset - 2);
                verticalOffset = Math.Min(relativeSourceLocation.Y + (this.PlacementTarget.ActualHeight / 2) - (this.PlacementTarget.ActualHeight / 2), bounds.Height - this.PlacementTarget.ActualHeight - 5);
                verticalOffset = Math.Max(5, verticalOffset - 1);

                // for entrance animation
                // UX guidelines show a PopIn animation
                Storyboard inAnimation = new Storyboard();
                PopInThemeAnimation popInThemeAnimation = new PopInThemeAnimation();

                switch (this.Placement)
                {
                    case Windows.UI.Popups.Placement.Left:
                        popInThemeAnimation.FromVerticalOffset = 0;
                        popInThemeAnimation.FromHorizontalOffset = 10;
                        horizondalOffset = relativeSourceLocation.X - 2;
                        break;
                    case Windows.UI.Popups.Placement.Below:
                        popInThemeAnimation.FromVerticalOffset = -10;
                        popInThemeAnimation.FromHorizontalOffset = 0;
                        verticalOffset = Math.Min(relativeSourceLocation.Y + this.PlacementTarget.ActualHeight - 1, bounds.Height - ((FrameworkElement)this.m_contextMenuPopup.Child).ActualHeight - 5);
                        break;
                    default:
                        popInThemeAnimation.FromVerticalOffset = 10;
                        popInThemeAnimation.FromHorizontalOffset = 0;
                        verticalOffset = Math.Max(5, relativeSourceLocation.Y - ((FrameworkElement)this.m_contextMenuPopup.Child).ActualHeight - 1);
                        break;
                }
                this.m_contextMenuPopup.HorizontalOffset = horizondalOffset;
                this.m_contextMenuPopup.VerticalOffset = verticalOffset;
                this.m_contextMenuPopup.Opacity = 1;

                Storyboard.SetTarget(popInThemeAnimation, this.m_contextMenuPopup);
                inAnimation.Children.Add(popInThemeAnimation);
                inAnimation.Begin();
            }
        }
        
        private void OnContextMenuClosed(object sender, object e)
        {
            this.IsOpen = false;
        }
        private void OnSelectorItemTapped(object sender, TappedRoutedEventArgs e)
        {
            this.m_contextMenuPopup.IsOpen = false;
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                if (menuItem.Command != null)
                {
                    menuItem.Command.Execute(menuItem.CommandParameter);
                }
            }
        }
        /// <summary>
        /// Represents the callback that is invoked when the effective property value of a dependency property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnIsOpenChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((ContextMenu)dependencyObject).OnIsOpenChanged(dependencyPropertyChangedEventArgs.OldValue, dependencyPropertyChangedEventArgs.NewValue);
        }
        #endregion

    }
}
