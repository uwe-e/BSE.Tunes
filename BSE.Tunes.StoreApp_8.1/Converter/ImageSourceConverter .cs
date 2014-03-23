using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.UI.Xaml.Media;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BSE.Tunes.StoreApp.Converter
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, System.Type type, object parameter, string language)
        {
            ImageSource src = null;
            byte[] bytearr = value as byte[];
            if (bytearr != null)
            {
				src = ConvertToImage(bytearr).Result;
            }
            return src;
        }
        public object ConvertBack(object value, System.Type type, object parameter, string language)
        {
            throw new NotImplementedException(); //doing one-way binding so this is not required.
        }
		public static async Task<BitmapImage> ConvertToImage(byte[] bytearr)
		{
			var image = new BitmapImage();
			using (MemoryStream memoryStream = new MemoryStream(bytearr))
			{
				IRandomAccessStream randomAccessStream = memoryStream.AsRandomAccessStream();
				if (randomAccessStream != null)
				{
					image.SetSourceAsync(randomAccessStream);
				}
			}
			return image;
		} 
    }
}
