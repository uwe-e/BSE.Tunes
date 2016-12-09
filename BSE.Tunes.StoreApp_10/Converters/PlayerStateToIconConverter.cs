using BSE.Tunes.Data.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BSE.Tunes.StoreApp.Converter
{
    public class PlayerStateToIconConverter : IValueConverter
    {
        public object Convert(object value, System.Type type, object parameter, string language)
        {
            PlayerState playerstate = (PlayerState)value;
                if (playerstate == PlayerState.Playing)
                {
                    return "";
            }
            return "";
        }
        public object ConvertBack(object value, System.Type type, object parameter, string language)
        {
            throw new NotImplementedException(); //doing one-way binding so this is not required.
        }
    }
}
