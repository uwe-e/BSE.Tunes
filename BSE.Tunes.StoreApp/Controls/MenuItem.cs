using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace BSE.Tunes.StoreApp.Controls
{
    /// <summary>
    /// Represents the container for an item in a ContextMenu control.
    /// </summary>
    public class MenuItem : ListViewItem
    {
        #region DependencyProperties
        /// <summary>
        /// Identifies the Placement dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(MenuItem), null);
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(MenuItem), null);
        #endregion

        #region Properties
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }
        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }
        #endregion

        #region MethodsPublic
        public MenuItem()
        {
            this.DefaultStyleKey = typeof(MenuItem);
            this.PointerEntered += (sender, args) => VisualStateManager.GoToState(this, "PointerOver", true);
            this.PointerExited += (sender, args) => VisualStateManager.GoToState(this, "Normal", true);
            this.PointerPressed += (sender, args) => VisualStateManager.GoToState(this, "Pressed", true);
            this.PointerReleased += (sender, args) => VisualStateManager.GoToState(this, "Normal", true);

        }
        #endregion
    }
}
