using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace BSE.Tunes.StoreApp.Controls
{
    [TemplatePart(Name = FadeInAnimationName, Type = typeof(Storyboard))]
    [TemplatePart(Name = FadeOutAnimationName, Type = typeof(Storyboard))]
    [TemplatePart(Name = ImageName, Type = typeof(Image))]
    public class CrossfadingImage : ContentControl
    {
        private const string ImageName = "Image";
        private const string FadeOutAnimationName = "ImageFadeOut";
        private const string FadeInAnimationName = "ImageFadeIn";

        #region FieldsPrivate
        private Image m_image;
        private Storyboard m_storyboardFadeOut;
        private Storyboard m_storyboardFadeIn;
        #endregion
        
        public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(ImageSource), typeof(CrossfadingImage),
            new PropertyMetadata(default(ImageSource), OnSourceChanged));
        
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register("Stretch", typeof(Stretch), typeof(CrossfadingImage),
            new PropertyMetadata(default(Stretch), null));

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CrossfadingImage), null);

        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CrossfadingImage), null);

        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        public CrossfadingImage()
        {
            this.DefaultStyleKey = typeof(CrossfadingImage);
            //See http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.frameworkelement.loaded
            // OnApplyTemplate vs. Loaded
            this.Loaded += (sender, e) =>
                {
                    if (this.m_image != null && this.m_image.Source == null)
                    {
                        this.m_image.Source = (Windows.UI.Xaml.Media.Imaging.BitmapImage)this.GetValue(SourceProperty);
                    }
                };
            this.Tapped += (sender, e) =>
                {
                    if (this.Command != null)
                    {
                        this.Command.Execute(this.CommandParameter);
                    }
                };
        }

        protected virtual void OnSourceChanged(object oldValue, object newValue)
        {
            if (this.m_image != null)
            {
                var bitmapImage = (Windows.UI.Xaml.Media.Imaging.BitmapImage)newValue;
                this.m_storyboardFadeOut.Completed += (s, e) =>
                    {
                        this.m_image.Source = bitmapImage;
                        this.m_storyboardFadeIn.Begin();
                    };
                this.m_storyboardFadeOut.Begin();
            }
        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.m_image = this.GetTemplateChild(ImageName) as Image;
            this.m_storyboardFadeOut = this.GetTemplateChild(FadeOutAnimationName) as Storyboard;
            this.m_storyboardFadeIn = this.GetTemplateChild(FadeInAnimationName) as Storyboard;
        }
        private static void OnSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((CrossfadingImage)dependencyObject).OnSourceChanged(dependencyPropertyChangedEventArgs.OldValue, dependencyPropertyChangedEventArgs.NewValue);
        }
    }
}
