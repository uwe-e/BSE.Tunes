using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Controls.Extensions
{
    /// <summary>
    /// Extension methods and attached properties for the <see cref="Windows.UI.Xaml.Controls.ContentDialog"/> class.
    /// </summary>
    public static class ContentDialog
    {
        /// <summary>
        /// Gets the identifier for the DialogCancel dependency property. 
        /// </summary>
        public static readonly DependencyProperty DialogCancelProperty =
            DependencyProperty.RegisterAttached("DialogCancel",
                typeof(bool),
                typeof(ContentDialog), new PropertyMetadata(false));
        /// <summary>
        /// Gets the identifier for the CancelableCommandParameter dependency property. 
        /// </summary>
        public static readonly DependencyProperty CancelableCommandParameterProperty =
            DependencyProperty.Register("CancelableCommandParameter",
                typeof(object),
                typeof(ContentDialog), null);
        /// <summary>
        /// Gets the identifier for the CancelableCommand dependency property. 
        /// </summary>
        public static readonly DependencyProperty CancelableCommandProperty =
            DependencyProperty.RegisterAttached("CancelableCommand",
                typeof(ICommand),
                typeof(ContentDialog),
                new PropertyMetadata(null, OnCancelableCommandChanged));
        /// <summary>
        /// Set the DialogCancel value.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="value">True to cancel the button click; Otherwise, false.</param>
        public static void SetDialogCancel(DependencyObject obj, bool value)
        {
            obj.SetValue(DialogCancelProperty, value);
        }
        /// <summary>
        /// Get the DialogCancel value.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <returns>True to cancel the button click; Otherwise, false.</returns>
        public static bool GetDialogCancel(DependencyObject obj)
        {
            return (bool)obj.GetValue(DialogCancelProperty);
        }
        /// <summary>
        /// Gets the cancelable command to invoke when this primary button is pressed.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <returns>The cancelable command to invoke when this primary button is pressed</returns>
        public static ICommand GetCancelableCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CancelableCommandProperty);
        }
        /// <summary>
        /// Sets the cancelable command to invoke when this primary button is pressed.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="value">The cancelable command to invoke when this primary button is pressed.</param>
        public static void SetCancelableCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CancelableCommandProperty, value);
        }
        /// <summary>
        /// Gets the parameter to pass to the CancelableCommand property.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <returns>The parameter to pass to the CancelableCommand property.</returns>
        public static object GetCancelableCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CancelableCommandParameterProperty);
        }
        /// <summary>
        /// Sets the parameter to pass to the CancelableCommand property.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="value">The parameter to pass to the CancelableCommand property.</param>
        public static void SetCancelableCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CancelableCommandParameterProperty, value);
        }

        private static void OnCancelableCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var contentDialog = obj as Windows.UI.Xaml.Controls.ContentDialog;
            if (contentDialog != null)
            {
                contentDialog.Loaded += (sender, routedEventArgs) =>
                {
                    ((Windows.UI.Xaml.Controls.ContentDialog)sender).PrimaryButtonClick += OnPrimaryButtonClick;
                };
            }
        }

        private static void OnPrimaryButtonClick(Windows.UI.Xaml.Controls.ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var contentDialog = sender as Windows.UI.Xaml.Controls.ContentDialog;
            if (contentDialog != null)
            {
                var command = GetCancelableCommand(contentDialog);
                command?.Execute(GetCancelableCommandParameter(contentDialog));
                args.Cancel = GetDialogCancel(contentDialog);
            }
        }
    }
}
