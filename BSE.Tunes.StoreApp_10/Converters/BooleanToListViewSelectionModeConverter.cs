using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace BSE.Tunes.StoreApp.Converters
{
    public sealed class BooleanToListViewSelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? ListViewSelectionMode.Multiple : ListViewSelectionMode.Extended;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is ListViewSelectionMode && (ListViewSelectionMode)value == ListViewSelectionMode.Multiple;
        }
    }
}
