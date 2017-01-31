using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.IO;

namespace WinRTXamlToolkit.Imaging
{
    /// <summary>
    /// Contains extension and helper methods for loading WriteableBitmaps.
    /// </summary>
    public static class WriteableBitmapLoadExtensions
    {
        /// <summary>
        /// Loads the WriteableBitmap asynchronously given the storage file.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="storageFile">The storage file.</param>
        /// <returns></returns>
        public static async Task<WriteableBitmap> LoadAsync(
            this WriteableBitmap writeableBitmap,
            StorageFile storageFile)
        {
            var wb = writeableBitmap;
            using (var stream = await storageFile.OpenReadAsync())
            {
                await wb.SetSourceAsync(stream);
            }
            return wb;
        }
    }
}
