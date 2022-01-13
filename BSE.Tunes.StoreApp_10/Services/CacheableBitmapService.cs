using BSE.Tunes.StoreApp.IO;
using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.Imaging;

namespace BSE.Tunes.StoreApp.Services
{
    public class CacheableBitmapService : ICacheableBitmapService
    {
        private const string ThumbnailPart = "_thumb";
        public static string ImageExtension => ".jpg";

        public static ICacheableBitmapService Instance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ICacheableBitmapService>() as CacheableBitmapService;
            }
        }

        public async Task<BitmapSource> GetBitmapSource(object bitmapSource, string cacheName, int width, bool asThumbnail = false)
        {
            WriteableBitmap writeableBitmap = null;
            var height = width;
            ObservableCollection<Uri> uriSource = bitmapSource as ObservableCollection<Uri>;
            if (!string.IsNullOrEmpty(cacheName) && uriSource != null)
            {
                try
                {
                    cacheName = asThumbnail ? cacheName + ThumbnailPart : cacheName;
                    string cacheFileName = $"t{cacheName}.{CacheableBitmapService.ImageExtension}";

                    var storageFolder = await LocalStorage.GetImageFolderAsync();

                    var storageFile = await storageFolder.TryGetItemAsync(cacheFileName) as StorageFile;
                    if (storageFile != null)
                    {
                        return writeableBitmap = await new WriteableBitmap(width, height).LoadAsync(storageFile);
                    }

                    List<StorageFile> storageFiles = await GetStorageFilesAsync(asThumbnail, uriSource, storageFolder);

                    var innerWidth = width / 2;
                    var innerHeight = innerWidth;
                    int index = 0;

                    writeableBitmap = new WriteableBitmap(width, height);

                    foreach (var file in storageFiles)
                    {
                        var randomAccessStreamReference = RandomAccessStreamReference.CreateFromFile(file);
                        using (IRandomAccessStream randomAccessStream = await randomAccessStreamReference.OpenReadAsync())
                        {
                            if (randomAccessStream != null)
                            {
                                //We initialize the bitmap with height and width, but the actual size will be reset after the FromStream method!
                                WriteableBitmap innerImage = new WriteableBitmap(innerWidth, innerHeight);
                                innerImage = await BitmapFactory.FromStream(randomAccessStream);

                                int x = 0;
                                int y = 0;

                                if (index == 1 || index == 2)
                                {
                                    x += innerWidth;
                                }
                                if (index == 1 || index == 3)
                                {
                                    y += innerHeight;
                                }

                                writeableBitmap.Blit(
                                    new Rect()
                                    {
                                        Height = innerHeight,
                                        Width = innerWidth,
                                        X = x,
                                        Y = y
                                    },
                                        innerImage,
                                        new Rect()
                                        {
                                            Height = innerImage.PixelHeight,
                                            Width = innerImage.PixelWidth,
                                            X = 0,
                                            Y = 0
                                        }, WriteableBitmapExtensions.BlendMode.Additive);

                                index++;
                            }
                        }
                    }
                    await writeableBitmap.SaveAsync(storageFolder, cacheFileName, CreationCollisionOption.ReplaceExisting);
                }
                catch (Exception)
                {

                }
            }
            return writeableBitmap;
        }
        public async Task<bool> RemoveCache(string cacheName)
        {
            bool hasRemoved = false;
            if (string.IsNullOrEmpty(cacheName) == false)
            {
                string cacheFileName = string.Format("{0}{1}", cacheName, ImageExtension);
                var storageFolder = await LocalStorage.GetImageFolderAsync();
                var storageFiles = await storageFolder.GetFilesAsync();
                if (storageFiles != null)
                {
                    var files = storageFiles.Where(file => file.Name.StartsWith(cacheName));
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                    }
                    hasRemoved = true;
                }
            }
            return hasRemoved;
        }

        private async Task<List<StorageFile>> GetStorageFilesAsync(bool asThumbnail, ObservableCollection<Uri> uriSource, StorageFolder storageFolder)
        {
            List<StorageFile> storageFiles = new List<StorageFile>();
            foreach (Uri uri in uriSource)
            {
                if (uri.Segments[4] == "image/")
                {
                    var segment = uri.Segments[5];
                    var imageName = segment.Substring(0, segment.Length - 1);
                    imageName = asThumbnail ? imageName + ThumbnailPart : imageName;
                    var imageFileName = $"{imageName}{ImageExtension}";

                    var imageFile = await GetStorageFileAsync(storageFolder, uri, imageFileName);
                    storageFiles.Add(imageFile);
                }
            }

            return storageFiles;
        }

        private async Task<StorageFile> GetStorageFileAsync(StorageFolder storageFolder, Uri uri, string imageFileName)
        {
            StorageFile storageFile = await storageFolder.TryGetItemAsync(imageFileName) as StorageFile;
            if (storageFile == null)
            {
                storageFile = await StorageFile.CreateStreamedFileFromUriAsync(imageFileName, uri, null);
                return await storageFile.CopyAsync(storageFolder, imageFileName, NameCollisionOption.ReplaceExisting);
            }
            return storageFile;
        }

    }
}
