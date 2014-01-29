using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace BSE.Net.Http
{
    public class AudioStream
    {
        private readonly string m_fileName;

        public AudioStream(string fileName)
        {
            this.m_fileName = fileName;
        }

        public async System.Threading.Tasks.Task WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                var buffer = new byte[65536];

                using (var audio = File.Open(m_fileName, FileMode.Open, FileAccess.Read))
                {
                    var length = (int)audio.Length;
                    var bytesRead = 1;

                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = audio.Read(buffer, 0, Math.Min(length, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        length -= bytesRead;
                    }
                }
            }
            catch (HttpException ex)
            {
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }
    }
}