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
                BitmapImage image = new BitmapImage();
                using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
                {
                    using (DataWriter dataWriter = new DataWriter(ms.GetOutputStreamAt(0)))
                    {
                        dataWriter.WriteBytes(bytearr);
                        dataWriter.StoreAsync().AsTask().Wait();
                        dataWriter.FlushAsync().AsTask().Wait();
                    }
                    image.SetSource(ms);
                }
                src = image;
            }
            return src;
        }
        public object ConvertBack(object value, System.Type type, object parameter, string language)
        {
            throw new NotImplementedException(); //doing one-way binding so this is not required.
        }
    }
}
