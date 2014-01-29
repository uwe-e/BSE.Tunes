using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Behaviours
{
    public class ImageExtensions
    {
        /// <summary>
        /// FadeInOnLoaded Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty FadeInProperty =
            DependencyProperty.RegisterAttached(
                "FadeIn",
                typeof(bool),
                typeof(ImageExtensions),
                new PropertyMetadata(false, OnFadeInOnLoadedChanged));
        /// <summary>
        /// Gets the FadeInOnLoaded property. This dependency property 
        /// indicates whether the image should be transparent and fade in into view only when loaded.
        /// </summary>
        public static bool GetFadeIn(DependencyObject d)
        {
            return (bool)d.GetValue(FadeInProperty);
        }

        /// <summary>
        /// Sets the FadeInOnLoaded property. This dependency property 
        /// indicates whether the image should be transparent and fade in into view only when loaded.
        /// </summary>
        public static void SetFadeIn(DependencyObject d, bool value)
        {
            d.SetValue(FadeInProperty, value);
        }
        /// <summary>
        /// Handles changes to the FadeInOnLoaded property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnFadeInOnLoadedChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool shouldFadeIn = (bool)d.GetValue(FadeInProperty);
            var image = d as Image;
            if (image == null)
            {
                return;
            }
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (shouldFadeIn)
            {
                image.Loaded += OnImageLoaded;
            }
        }
        static void OnImageLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
