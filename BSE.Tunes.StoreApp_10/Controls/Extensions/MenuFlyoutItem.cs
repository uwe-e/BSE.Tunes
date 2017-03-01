using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BSE.Tunes.StoreApp.Controls.Extensions
{
    /// <summary>
    /// Extension methods and attached properties for the <see cref="Windows.UI.Xaml.Controls.MenuFlyoutItem"/> class.
    /// </summary>
    public static class MenuFlyoutItem
    {
        /// <summary>
        /// Gets the identifier for the UpdateTagTarget dependency property. 
        /// </summary>
        public static readonly DependencyProperty UpdateTagTargetProperty =
            DependencyProperty.RegisterAttached("UpdateTagTarget",
                typeof(bool),
                typeof(MenuFlyoutItem),
                new PropertyMetadata(false, OnUpdateTargetChanged));
        /// <summary>
        /// Set the UpdateTagTarget value.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="value">True to update the target source; Otherwise, false.</param>
        public static void SetUpdateTagTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(UpdateTagTargetProperty, value);
        }
        // <summary>
        /// Get the UpdateTagTarget value.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <returns>True to update the target source; Otherwise, false.</returns>
        public static bool GetUpdateTagTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(UpdateTagTargetProperty);
        }
        private static void OnUpdateTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menuFlyoutItem = d as Windows.UI.Xaml.Controls.MenuFlyoutItem;
            if (menuFlyoutItem != null)
            {
                menuFlyoutItem.Loaded += (sender, routedEventArgs) =>
                {
                    ((Windows.UI.Xaml.Controls.MenuFlyoutItem)sender).GetBindingExpression(Windows.UI.Xaml.Controls.MenuFlyoutItem.TagProperty).UpdateSource();
                };
            }
        }
    }
}
