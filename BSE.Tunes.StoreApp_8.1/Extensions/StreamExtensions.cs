using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BSE.Tunes.StoreApp.Extensions
{
    public static class StreamExtensions
    {
        public async static Task<IRandomAccessStream> AsRandomAccessStreamAsync(this Stream stream)
        {
            Stream streamToConvert = null;

            if (!stream.CanRead)
            {
                throw new Exception("Cannot read the source stream-");
            }
            if (!stream.CanSeek)
            {
                MemoryStream memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                streamToConvert = memoryStream;
            }
            else
            {
                streamToConvert = stream;
            }

            DataReader dataReader = new DataReader(streamToConvert.AsInputStream());
            streamToConvert.Position = 0;
            await dataReader.LoadAsync((uint)streamToConvert.Length);
            IBuffer buffer = dataReader.ReadBuffer((uint)streamToConvert.Length);

            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            IOutputStream outputstream = randomAccessStream.GetOutputStreamAt(0);
            await outputstream.WriteAsync(buffer);
            await outputstream.FlushAsync();

            return randomAccessStream;
        }
    }
}
