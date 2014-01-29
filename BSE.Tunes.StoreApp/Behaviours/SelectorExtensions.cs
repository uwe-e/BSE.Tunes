using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace BSE.Tunes.StoreApp.Behaviours
{
    public static class SelectorExtensions
    {
        #region ItemClick Command
        /// <summary>
        /// ItemToBringIntoView Attached Command Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
            typeof(ICommand),
            typeof(SelectorExtensions),
            new PropertyMetadata(null, OnCommandChanged));
        /// <summary>
        /// Gets the command to invoke when an item is clicked.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>The invoked command.</returns>
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }
        /// <summary>
        /// Sets the command to invoke when an item is clicked.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value">The command.</param>
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }
        /// <summary>
        /// Handles changes to the Command property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Selector;
            if (control != null)
            {
                control.SelectionChanged += OnSelectionChanged;
            }
        }
        /// <summary>
        /// Occurs when the currently selected item changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            var control = sender as Selector;
            if (control != null)
            {
                var command = GetCommand(control);
                var parameter = GetCommandParameter(control);
                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter",
            typeof(object),
            typeof(SelectorExtensions),
            new PropertyMetadata(null));

        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }
        #endregion
    }
}
