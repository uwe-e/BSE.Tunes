using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace Template10.Services.NavigationService
{
    public partial class NavigationService : BSE.Tunes.StoreApp.Services.INavigationService
    {
        public Task<bool> NavigateAsync(Type page, object parameter = null, bool isFullScreen = false, NavigationTransitionInfo infoOverride = null)
        {
            return Task.Run(() =>
            {
                return false;
            }
            );
        }
    }
}
