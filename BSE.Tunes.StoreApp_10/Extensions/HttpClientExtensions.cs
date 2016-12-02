using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Extensions
{
    public static class HttpClientExtensions
    {
        public static void AddRange(this HttpClient request, long from, long to)
        {
            if (request != null)
            {
                request.DefaultRequestHeaders.Range = new RangeHeaderValue(from, to);
            }
        }
    }
}
