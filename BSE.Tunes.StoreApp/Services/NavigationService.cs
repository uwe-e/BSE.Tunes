using BSE.Tunes.StoreApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Services
{
    public class NavigationService : INavigationService
    {
        #region FieldsPrivate
        private readonly Frame m_rootFrame;
        #endregion

        #region MethodsPublic
        /// <summary>
        /// Initializes a new instance of the NavigationService class.
        /// </summary>
        public NavigationService()
        {
        }
        /// <summary>
        /// Initializes a new instance of the NavigationService class.
        /// </summary>
        /// <param name="rootFrame">The content control that supports navigation</param>
        public NavigationService(Frame rootFrame)
        {
            this.m_rootFrame = rootFrame;
            this.m_rootFrame.Navigating += OnFrameNavigating;
            this.m_rootFrame.Navigated += OnFrameNavigated;
        }
        /// <summary>
        /// Causes the Frame to load content represented by the specified Page-derived data type.
        /// </summary>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        public void Navigate(Type sourcePageType)
        {
            if (this.m_rootFrame != null)
            {
                this.Navigate(sourcePageType, null);
            }
        }
        /// <summary>
        /// Causes the Frame to load content represented by the specified Page-derived data type, also passing a parameter to be interpreted by the target of the navigation.
        /// </summary>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        public void Navigate(Type sourcePageType, object parameter)
        {
            if (this.m_rootFrame != null)
            {
                this.m_rootFrame.Navigate(sourcePageType, parameter);
            }
        }
        /// <summary>
        /// Navigates to the most recent item in back navigation history.
        /// </summary>
        public void GoBack()
        {
            if (this.m_rootFrame != null && this.m_rootFrame.CanGoBack)
            {
                this.m_rootFrame.GoBack();
            }
        }
        /// <summary>
        /// Used for navigating away from the current view model due to a suspension event, in this way you can execute additional logic to handle suspensions.
        /// </summary>
        public void Suspending()
        {
            this.NavigateFromCurrentViewModel(true);
        }
        #endregion

        #region MethodsPrivate
        private void OnFrameNavigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            this.NavigateFromCurrentViewModel(false);
        }
        private void OnFrameNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var newView = this.m_rootFrame.Content as FrameworkElement;
            if (newView != null)
            {
                var newViewModel = newView.DataContext as INavigationAware;
                if (newViewModel != null)
                {
                    newViewModel.OnNavigatedTo(e.Parameter, e.NavigationMode);
                }
            }
        }
         /// <summary>
        /// Navigates away from the current viewmodel.
        /// </summary>
        /// <param name="suspending">True if it is navigating away from the viewmodel due to a suspend event.</param>
        private void NavigateFromCurrentViewModel(bool suspending)
        {
            var oldView = this.m_rootFrame.Content as FrameworkElement;
            if (oldView != null)
            {
                var oldViewModel = oldView.DataContext as INavigationAware;
                if (oldViewModel != null)
                {
                    oldViewModel.OnNavigatedFrom(suspending);
                }
            }
        }
        #endregion
    }
}
