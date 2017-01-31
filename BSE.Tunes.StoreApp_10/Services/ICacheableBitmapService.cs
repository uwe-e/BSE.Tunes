using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace BSE.Tunes.StoreApp.Services
{
    public interface ICacheableBitmapService
    {
        Task<BitmapSource> GetBitmapSource(object bitmapSource, string cacheName, int width, bool asThumbnail = false);
        Task<bool> RemoveCache(string cacheName);
    }
}
