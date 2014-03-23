using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
namespace BSE.Tunes.StoreApp.Views
{
    public sealed partial class ImagePreview : UserControl
    {
        #region DependencyProperties
        public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImagePreview),
            new PropertyMetadata(default(ImageSource), SourceChanged));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ImagePreview),
            new PropertyMetadata(default(Stretch), null));

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
        public static readonly DependencyProperty ImageOpacityProperty =
            DependencyProperty.Register("ImageOpacity", typeof(double), typeof(ImagePreview),
            new PropertyMetadata(1));
        
        public double ImageOpacity
        {
            get { return (double)this.GetValue(ImageOpacityProperty); }
            set { this.SetValue(ImageOpacityProperty, value); }
        }
        
        #endregion

        #region MethodsPublic
        public ImagePreview()
        {
            this.InitializeComponent();
            this.Opacity = 100;
        }
        #endregion

        #region MethodsPrivate
        private static void SourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ImagePreview)dependencyObject;
            var newSource = (ImageSource)dependencyPropertyChangedEventArgs.NewValue;
            if (newSource != null)
            {
                control.LoadImage(newSource);
            }
        }

        private void LoadImage(ImageSource source)
        {
            //var timer = new DispatcherTimer();
            //    timer.Interval = TimeSpan.FromSeconds(2);
            //    timer.Tick += (o, o1) =>
            //    {
            //        //ImageFadeOut.Stop();
            //        Image.Source = source;
            //        ImageFadeIn.Begin();
            //        timer.Stop();
            //    };
            //    timer.Start();
            //    ImageFadeOut.Begin();
            ImageFadeOut.Completed += (s, e) =>
            {
                Image.Source = source;
                ImageFadeIn.Begin();
            };
            ImageFadeOut.Begin();
        }
        #endregion
    }
}
