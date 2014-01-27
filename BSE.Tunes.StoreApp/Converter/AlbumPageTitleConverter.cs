using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BSE.Tunes.StoreApp.Converter
{
    public class AlbumPageTitleConverter : IValueConverter
    {
        public object Convert(object value, System.Type type, object parameter, string language)
        {
            string strValue = null;
            Album album = value as Album;
            if (album != null)
            {
                strValue = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", album.Title, album.Artist.Name);
            }
            return strValue;
        }
        public object ConvertBack(object value, System.Type type, object parameter, string language)
        {
            throw new NotImplementedException(); //doing one-way binding so this is not required.
        }
    }
}
