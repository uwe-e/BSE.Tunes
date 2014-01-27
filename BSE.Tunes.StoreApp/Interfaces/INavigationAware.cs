using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.Interfaces
{
    /// <summary>
    /// The INavigationAware interface should be used for view models that require persisting and loading state due to suspend/resume events.
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Called when navigation is performed to a page. You can use this method to load state if it is available.
        /// </summary>
        /// <param name="navigationParameter">The navigation parameter.</param>
        /// <param name="navigationMode">The navigation mode.</param>
        void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode);
        /// <summary>
        /// This method will be called when navigating away from a page. You can use this method to save your view model data in case of a suspension event.
        /// </summary>
        /// <param name="suspending">if set to <c>true</c> you are navigating away of this viewmodel due to a suspension event.</param>
        void OnNavigatedFrom(bool suspending);
    }
}
