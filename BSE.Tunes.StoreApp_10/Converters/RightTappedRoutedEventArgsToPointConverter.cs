using BSE.Tunes.StoreApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace BSE.Tunes.StoreApp.Converters
{
    public sealed class RightTappedRoutedEventArgsToPointConverter : IValueConverter
    {
        public object Convert(object value, System.Type type, object parameter, string language)
        {
            IPlaceableContext placeableContext = null;
            var args = value as RightTappedRoutedEventArgs;
            var frameworkElement = parameter as FrameworkElement;
            if (frameworkElement != null)
            {
                placeableContext = frameworkElement.DataContext as IPlaceableContext;
                placeableContext.OffsetPoint = args?.GetPosition(frameworkElement) ?? new Point();
            }
            return placeableContext;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException(); //doing one-way binding so this is not required.
        }
    }
}
