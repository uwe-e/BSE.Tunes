using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace BSE.Tunes.StoreApp.Controls
{
    public class DropDownButton : Button
    {
        #region DependencyProperties
        /// <summary>
        /// Menu dependency property
        /// </summary>
        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.Register("ContextMenu", typeof(ContextMenu), typeof(DropDownButton), new PropertyMetadata(null));
        /// <summary>
        /// Identifies the Placement dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register("Placement", typeof(Placement), typeof(DropDownButton), new PropertyMetadata(Placement.Default));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the menu
        /// </summary>
        public ContextMenu ContextMenu
        {
            get { return (ContextMenu)this.GetValue(ContextMenuProperty); }
            set { this.SetValue(ContextMenuProperty, value); }
        }
        /// <summary>
        /// Gets or sets the orientation of the Popup control when the control opens, and specifies the behavior of the Popup control when it overlaps screen boundaries.
        /// </summary>
        public Placement Placement
        {
            get { return (Placement)this.GetValue(PlacementProperty); }
            set { this.SetValue(PlacementProperty, value); }
        }
        #endregion

        #region MethodsPublic
        public DropDownButton()
        {
            //this.DefaultStyleKey = typeof(DropDownButton);
        }
        #endregion

        #region MethodsProtected
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            if (this.ContextMenu != null)
            {
                this.ContextMenu.PlacementTarget = this;
                this.ContextMenu.Placement = this.Placement;
                this.ContextMenu.DataContext = this.DataContext;
                this.ContextMenu.IsOpen = true;
            }
        }
        #endregion

        #region MethodsPrivate
        #endregion
    }
}
