using BSE.Tunes.StoreApp.IO;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.Imaging;

namespace BSE.Tunes.StoreApp.Services
{
    public class CacheableBitmapService :  ICacheableBitmapService
    {
        #region Constants
        private const string ThumbnailPart = "_thumb";
        private const string ImageExtension = ".jpg";
        #endregion

        #region MethodsPublic
        public static ICacheableBitmapService Instance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ICacheableBitmapService>() as CacheableBitmapService;
            }
        }
        public async Task<BitmapSource> GetBitmapSource(object bitmapSource, string cacheName, int width, bool asThumbnail = false)
		{
			var height = width;
			WriteableBitmap writeableBitmap = null;
			ObservableCollection<Uri> uriSource = bitmapSource as ObservableCollection<Uri>;
			if (!string.IsNullOrEmpty(cacheName) && uriSource != null)
			{
				try
				{
					cacheName = asThumbnail ? cacheName + ThumbnailPart : cacheName;
					string cacheFileName = string.Format("{0}.{1}", cacheName, ImageExtension);
					var storageFolder = await LocalStorage.GetImageFolderAsync();
					var storageFile = await storageFolder.TryGetItemAsync(cacheFileName) as StorageFile;
					if (storageFile != null)
					{
						writeableBitmap = await new WriteableBitmap(width, height).LoadAsync(storageFile);
					}

					var innerWidth = width / 2;
					var innerHeight = innerWidth;
					writeableBitmap = new WriteableBitmap(width, height);
					int index = 0;
					foreach (Uri uri in uriSource)
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
									if (index == 1 || index == 3)
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
						catch (Exception)
						{ }
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
                string cacheFileName = string.Format("{0}.{1}", cacheName, ImageExtension);
                var storageFolder = await LocalStorage.GetImageFolderAsync();
                var storageFiles = await storageFolder.GetFilesAsync();
                if (storageFiles != null)
                {
                    var files = storageFiles.Where(file => file.Name.StartsWith(cacheName));
                    foreach(var file in files)
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
        #endregion
    }
}
