using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Services
{
    public interface INavigationService
    {
        void Navigate(Type sourcePageType);
        void Navigate(Type sourcePageType, object parameter);
        void GoBack();
        /// <summary>
        /// Used for navigating away from the current view model due to a suspension event, in this way you can execute additional logic to handle suspensions.
        /// </summary>
        void Suspending();
    }
}
