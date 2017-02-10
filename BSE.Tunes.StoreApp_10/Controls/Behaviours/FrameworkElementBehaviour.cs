using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace BSE.Tunes.StoreApp.Controls.Behaviours
{
    public static class FrameworkElementBehaviour
    {
        /// <summary>
        /// Attached Command Property
        /// </summary>
        public static readonly DependencyProperty RightTappedCommandProperty =
            DependencyProperty.RegisterAttached("RightTappedCommand",
            typeof(ICommand),
            typeof(FrameworkElementBehaviour),
            new PropertyMetadata(null, OnRightTappedCommandChanged));
        /// <summary>
        /// Gets the command to invoke when an item is clicked.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>The invoked command.</returns>
        public static ICommand GetRightTappedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(RightTappedCommandProperty);
        }
        /// <summary>
        /// Sets the command to invoke when an item is clicked.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value">The command.</param>
        public static void SetRightTappedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(RightTappedCommandProperty, value);
        }
        /// <summary>
        /// Handles changes to the Command property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnRightTappedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FrameworkElement;
            if (control != null)
            {
                control.RightTapped += OnRightTapped;
                //control.RightTapped += Control_RightTapped;
            }
        }

        //private static void Control_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Occurs when an item in the list view receives an interaction, and the IsItemClickEnabled property is true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control != null)
            {
                FrameworkElement element = e.OriginalSource as FrameworkElement;
                if (element != null && element.DataContext != null)
                {
                    var command = GetRightTappedCommand(control);
                    command?.Execute(element.DataContext);
                }
            }
        }

    }
}
