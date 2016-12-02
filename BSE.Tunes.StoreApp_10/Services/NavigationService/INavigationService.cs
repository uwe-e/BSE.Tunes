using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace BSE.Tunes.StoreApp.Services
{
    public interface INavigationService
    {
        Task<bool> NavigateAsync(Type page, object parameter = null, bool isFullScreen = false, NavigationTransitionInfo infoOverride = null);
    }
}
