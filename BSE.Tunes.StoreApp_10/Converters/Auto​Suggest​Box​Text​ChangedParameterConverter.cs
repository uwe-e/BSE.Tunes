using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace BSE.Tunes.StoreApp.Converters
{
    public class Auto​Suggest​Box​Text​ChangedParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // cast value to whatever EventArgs class you are expecting here
            var args = (Auto​Suggest​Box​Text​Changed​Event​Args)value;
            // return what you need from the args
            //if (args..ChosenSuggestion != null)
            //{
            //    return args.ChosenSuggestion;
            //}
            //return args.QueryText;
            return "hallo";
        }
        public object ConvertBack(object value, System.Type type, object parameter, string language)
        {
            throw new NotImplementedException(); //doing one-way binding so this is not required.
        }
    }
}
