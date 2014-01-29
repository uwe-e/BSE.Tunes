using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BSE.Tunes.StoreApp.Converter
{
    public class CommaSeparatorConverter : IValueConverter
    {
        public object Convert(object value, System.Type type, object parameter, string language)
        {
            string strValue = null;
            if (value is int)
            {
                strValue = value.ToString();
            }
            if (value is string)
            {
                strValue = value.ToString();
            }
            if (string.IsNullOrEmpty(strValue) == false)
            {
                strValue = string.Format(CultureInfo.CurrentUICulture, ", {0}", strValue);
            }
            return strValue;
        }
        public object ConvertBack(object value, System.Type type, object parameter, string language)
        {
            throw new NotImplementedException(); //doing one-way binding so this is not required.
        }
    }
}
