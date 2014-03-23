using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace BSE.Tunes.StoreApp.Converter
{
    /// <summary>
    /// Value converter that translates a <see cref="Track"/> which is not null to <see cref="Visibility.Visible"/> and null to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class TrackToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((Track)value == null) ? Visibility.Collapsed : Visibility.Visible; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
