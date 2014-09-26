using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Services
{
    public interface INavigationService
    {
        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation history. 
        /// </summary>
        bool CanGoBack { get; }
        /// <summary>
        /// Causes the Frame to load content represented by the specified Page-derived data type.
        /// </summary>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <returns>false if a NavigationFailed event handler has set Handled to true; otherwise, true.</returns>
        bool Navigate(Type sourcePageType);
        /// <summary>
        /// Causes to load content represented by the specified source- and masterpage- types into the frames.
        /// </summary>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <param name="masterPageType">The masterpage type in which the content should be loaded.</param>
        /// <returns>false if a NavigationFailed event handler has set Handled to true; otherwise, true.</returns>
        bool Navigate(Type sourcePageType, Type masterPageType);
        /// <summary>
        /// Causes to load content represented by the specified source- and masterpage- types into the frames.
        /// </summary>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <param name="masterPageType">The masterpage type in which the content should be loaded.</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        /// <returns>false if a NavigationFailed event handler has set Handled to true; otherwise, true.</returns>
        bool Navigate(Type sourcePageType, Type masterPageType, object parameter);
        /// <summary>
        /// Navigates to the most recent item in back navigation history.
        /// </summary>
        void GoBack();
        /// <summary>
        /// Used for navigating away from the current view model due to a suspension event, in this way you can execute additional logic to handle suspensions.
        /// </summary>
        void Suspending();
    }
}
