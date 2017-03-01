using BSE.Tunes.StoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BSE.Tunes.StoreApp.Converters
{
    public class InsertModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string param = parameter as string;

            if (param == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return DependencyProperty.UnsetValue;
            }

            return Enum.Parse(value.GetType(), param);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string param = parameter as string;

            if (parameter == null)
            {
                return DependencyProperty.UnsetValue;
            }
            return Enum.Parse(typeof(InsertMode), param);
        }
    }
}
