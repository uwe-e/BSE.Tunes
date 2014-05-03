using BSE.Tunes.StoreApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.Imaging;

namespace BSE.Tunes.StoreApp.Controls
{
    [TemplatePart(Name = ImageName, Type = typeof(Image))]
    public class WriteableImage : ContentControl
    {
        #region Constants
        private const string ImageName = "Image";
        private const string ImageExtension = ".jpg";
        #endregion

        #region FieldsPrivate
        private Image m_image;
        private ObservableCollection<Uri> m_uriCollection;
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty ImagesSourceProperty =
            DependencyProperty.Register("ImagesSource", typeof(object), typeof(WriteableImage), new PropertyMetadata(default(object), OnImagesChanged));
        public static readonly DependencyProperty CacheNameProperty =
            DependencyProperty.Register("CacheName", typeof(string), typeof(WriteableImage), new PropertyMetadata(null, OnCacheNameChanged));
        /// <summary>
        /// Identifies the <see cref="IsCacheRemoved"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCacheRemovedProperty =
            DependencyProperty.Register("IsCacheRemoved", typeof(Boolean), typeof(WriteableImage), new PropertyMetadata(false, OnIsCacheRemovedChanged));
        #endregion

        #region Properties
        public object ImagesSource
        {
            get { return this.GetValue(ImagesSourceProperty); }
            set { this.SetValue(ImagesSourceProperty, value); }
        }
        public string CacheName
        {
            get { return (string)this.GetValue(CacheNameProperty); }
            set { this.SetValue(CacheNameProperty, value); }
        }
        public Boolean IsCacheRemoved
        {
            get { return (Boolean)this.GetValue(IsCacheRemovedProperty); }
            set { this.SetValue(IsCacheRemovedProperty, value); }
        }
        #endregion

        #region MethodsPublic
        public WriteableImage()
        {
            this.DefaultStyleKey = typeof(WriteableImage);
            //See http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.frameworkelement.loaded
            // OnApplyTemplate vs. Loaded
            this.Loaded += (sender, e) =>
            {
                if (this.m_image != null && this.m_image.Source == null)
                {
                    this.LoadImages();
                }
            };
        }
        public async void RemoveCache()
        {
            if (string.IsNullOrEmpty(this.CacheName) == false)
            {
                string cacheFileName = string.Format("{0}.{1}", this.CacheName, ImageExtension);
                var storageFile = await Windows.Storage.ApplicationData.Current.LocalFolder.TryGetItemAsync(cacheFileName) as StorageFile;
                if (storageFile != null)
                {
                    await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
        }
        public void RemoveCache(string fileName)
        {
            this.CacheName = fileName;
            if (string.IsNullOrEmpty(this.CacheName) == false)
            {
                RemoveCache();
            }
        }
        #endregion

        #region MethodsProtected
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.m_image = this.GetTemplateChild(ImageName) as Image;
        }
        protected virtual void OnImagesChanged(object e)
        {
            this.m_uriCollection = e as ObservableCollection<Uri>;
            if (this.m_uriCollection != null)
            {
                this.m_uriCollection.CollectionChanged += (sender, args) =>
                    {

                    };
                this.LoadImages();
            }
        }
        protected virtual void OnCacheNameChanged(string cacheName)
        {
            this.LoadImages();
        }
        protected virtual void OnIsCacheRemovedChanged(object oldValue, object newValue)
        {
            if ((bool)newValue)
            {
                bool isCacheRemoved = (Boolean)newValue;
                if (isCacheRemoved == true)
                {
                    this.RemoveCache();
                    this.IsCacheRemoved = false;
                }
            }
        }
        #endregion

        #region MethodsPrivate
        private async Task<BitmapSource> GetWriteableBitmap()
        {
            var width = (int)this.Width;
            var height = width;
            WriteableBitmap writeableBitmap = null;
            string cacheFileName = string.Format("{0}.{1}", this.CacheName, ImageExtension);
            var storageFile = await Windows.Storage.ApplicationData.Current.LocalFolder.TryGetItemAsync(cacheFileName) as StorageFile;
            if (storageFile != null)
            {
                writeableBitmap = await new WriteableBitmap(width, height).LoadAsync(storageFile);
            }
            else
            {
                try
                {
                    var innerWidth = width / 2;
                    var innerHeight = innerWidth;
                    writeableBitmap = new WriteableBitmap(width, height);
                    int index = 0;
                    foreach (Uri uri in this.m_uriCollection)
                    {
                        try
                        {
                            var randomAccessStreamReference = RandomAccessStreamReference.CreateFromUri(uri);
                            using (IRandomAccessStream randomAccessStream = await randomAccessStreamReference.OpenReadAsync())
                            {
                                if (randomAccessStream != null)
                                {
                                    //We initialize the bitmap with height and width, but the actual size will be reset after the FromStream method!
                                    WriteableBitmap innerImage = new WriteableBitmap(innerWidth, innerHeight);
                                    innerImage = await innerImage.FromStream(randomAccessStream);

                                    int xPosition = 0;
                                    int yPosition = 0;

                                    if (index == 1 || index == 2)
                                    {
                                        xPosition = xPosition + innerWidth;
                                    }
                                    if (index == 2 || index == 3)
                                    {
                                        yPosition = yPosition + innerHeight;
                                    }

                                    writeableBitmap.Blit(
                                        new Rect()
                                        {
                                            Height = innerHeight,
                                            Width = innerWidth,
                                            X = xPosition,
                                            Y = yPosition
                                        },
                                            innerImage,
                                            new Rect()
                                            {
                                                Height = innerImage.PixelHeight,
                                                Width = innerImage.PixelWidth,
                                                X = 0,
                                                Y = 0
                                            }, WriteableBitmapExtensions.BlendMode.Additive);
                                }
                                index++;
                            }
                        }
                        catch(Exception)
                        { }
                    }
                    await writeableBitmap.SaveToFile(Windows.Storage.ApplicationData.Current.LocalFolder, cacheFileName, CreationCollisionOption.ReplaceExisting);
                }
                catch (Exception)
                {
                }
            }
            return writeableBitmap;
        }
        private async void LoadImages()
        {
            if (this.m_image != null && this.ImagesSource != null && !string.IsNullOrEmpty(this.CacheName))
            {
                this.m_image.Source = await GetWriteableBitmap();
            }
            
            //if (string.IsNullOrEmpty(this.CacheName) == false)
            //{
            //    var width = (int)this.Width;
            //    var height = width;
            //    WriteableBitmap writeableBitmap = null;
            //    string cacheFileName = string.Format("{0}.{1}", this.CacheName, ImageExtension);
            //    var storageFile = await Windows.Storage.ApplicationData.Current.LocalFolder.TryGetItemAsync(cacheFileName) as StorageFile;
            //    if (storageFile != null)
            //    {
            //        writeableBitmap = await new WriteableBitmap(width, height).LoadAsync(storageFile);
            //        //this.m_image.Source = await new WriteableBitmap(width, height).LoadAsync(storageFile); ;
            //    }
            //    else
            //    {
            //        try
            //        {
            //            var innerWidth = width / 2;
            //            var innerHeight = innerWidth;
            //            writeableBitmap = new WriteableBitmap(width, height);
            //            int index = 0;
            //            foreach (Uri uri in this.m_uriCollection)
            //            {
            //                var randomAccessStreamReference = RandomAccessStreamReference.CreateFromUri(uri);
            //                using (IRandomAccessStream randomAccessStream = await randomAccessStreamReference.OpenReadAsync())
            //                {
            //                    if (randomAccessStream != null)
            //                    {
            //                        //We initialize the bitmap with height and width, but the actual size will be reset after the FromStream method!
            //                        WriteableBitmap innerImage = new WriteableBitmap(innerWidth, innerHeight);
            //                        innerImage = await innerImage.FromStream(randomAccessStream);

            //                        int xPosition = 0;
            //                        int yPosition = 0;

            //                        if (index == 1 || index == 2)
            //                        {
            //                            xPosition = xPosition + innerWidth;
            //                        }
            //                        if (index == 2 || index == 3)
            //                        {
            //                            yPosition = yPosition + innerHeight;
            //                        }

            //                        writeableBitmap.Blit(
            //                            new Rect()
            //                            {
            //                                Height = innerHeight,
            //                                Width = innerWidth,
            //                                X = xPosition,
            //                                Y = yPosition
            //                            },
            //                                innerImage,
            //                                new Rect()
            //                                {
            //                                    Height = innerImage.PixelHeight,
            //                                    Width = innerImage.PixelWidth,
            //                                    X = 0,
            //                                    Y = 0
            //                                }, WriteableBitmapExtensions.BlendMode.Additive);
            //                    }
            //                    index++;
            //                }
            //            }
            //            //this.m_image.Source = writeableBitmap;
            //            await writeableBitmap.SaveToFile(Windows.Storage.ApplicationData.Current.LocalFolder, cacheFileName, CreationCollisionOption.ReplaceExisting);
            //            //this.m_image.Source = writeableBitmap;
            //        }
            //        catch(Exception)
            //        { }
            //    }
            //    if (writeableBitmap != null)
            //    {
            //        this.m_image.Source = writeableBitmap;
            //    }
            //}
        }
        /// <summary>
        /// Represents the callback that is invoked when the effective property value of a dependency property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnImagesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((WriteableImage)dependencyObject).OnImagesChanged(dependencyPropertyChangedEventArgs.NewValue);
        }
        private static void OnCacheNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((WriteableImage)dependencyObject).OnCacheNameChanged((string)dependencyPropertyChangedEventArgs.NewValue);
        }
        /// <summary>
        /// Represents the callback that is invoked when the effective property value of a dependency property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnIsCacheRemovedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((WriteableImage)dependencyObject).OnIsCacheRemovedChanged(dependencyPropertyChangedEventArgs.OldValue, dependencyPropertyChangedEventArgs.NewValue);
        }
        #endregion
    }
}
