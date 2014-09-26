using BSE.Tunes.StoreApp.Interfaces;
using BSE.Tunes.StoreApp.Navigation;
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
        private List<PageStackEntry> m_backStack;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the root <see cref="Frame"/> of the app.
        /// </summary>
        public Frame RootFrame
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the content <see cref="Frame"/> of the app.
        /// The <see cref="ContentFrame"/> is used in association with masterpages.
        /// </summary>
        public Frame ContentFrame
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation history. 
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                return this.Backstack.Count > 1;
            }
        }
        /// <summary>
        /// Gets a collection of <see cref="PageStackEntry"/> instances representing the backward navigation history.
        /// </summary>
        public IList<PageStackEntry> Backstack
        {
            get
            {
                if (this.m_backStack == null)
                {
                    this.m_backStack = new List<PageStackEntry>();
                }
                return this.m_backStack;
            }
        }
        #endregion

        #region MethodsPublic
        /// <summary>
        /// Initializes a new instance of the NavigationService class.
        /// </summary>
        /// <param name="rootFrame">The content control that supports navigation</param>
        public NavigationService(Frame rootFrame)
        {
            this.RootFrame = rootFrame;
            this.RootFrame.Navigating += OnFrameNavigating;
            this.RootFrame.Navigated += OnFrameNavigated;
        }
        /// <summary>
        /// Causes the Frame to load content represented by the specified Page-derived data type.
        /// </summary>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <returns>false if a NavigationFailed event handler has set Handled to true; otherwise, true.</returns>
        public bool Navigate(Type sourcePageType)
        {
            return this.Navigate(sourcePageType, null);
        }
        /// <summary>
        /// Causes to load content represented by the specified source- and masterpage- types into the frames.
        /// </summary>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <param name="masterPageType">The masterpage type in which the content should be loaded.</param>
        /// <returns>false if a NavigationFailed event handler has set Handled to true; otherwise, true.</returns>
        public bool Navigate(Type sourcePageType, Type masterPageType)
        {
            return this.Navigate(sourcePageType, masterPageType, null);
        }
        /// <summary>
        /// Causes to load content represented by the specified source- and masterpage- types into the frames.
        /// </summary>
        /// <param name="sourcePageType">The data type of the content to load.</param>
        /// <param name="masterPageType">The masterpage type in which the content should be loaded.</param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        /// <returns>false if a NavigationFailed event handler has set Handled to true; otherwise, true.</returns>
        public bool Navigate(Type sourcePageType, Type masterPageType, object parameter)
        {
            this.Backstack.Add(new PageStackEntry(sourcePageType, masterPageType, parameter));
            return this.NavigateImpl(sourcePageType, masterPageType, parameter);
        }
        /// <summary>
        /// Navigates to the most recent item in back navigation history.
        /// </summary>
        public void GoBack()
        {
            if (this.CanGoBack)
            {
                int iLastIndex = this.Backstack.Count - 1;
                if (iLastIndex > 0)
                {
                    int iPreLastIndex = iLastIndex - 1;
                    PageStackEntry pageStackEntry = this.Backstack[iPreLastIndex];
                    if (pageStackEntry != null)
                    {
                        this.NavigateImpl(pageStackEntry.SourcePageType, pageStackEntry.MasterPageType, pageStackEntry.Parameter);
                        this.Backstack.RemoveAt(iLastIndex);
                    }
                }
            }
        }
        /// <summary>
        /// Used for navigating away from the current view model due to a suspension event, in this way you can execute additional logic to handle suspensions.
        /// </summary>
        public void Suspending()
        {
            if (this.ContentFrame != null)
            {
                this.ContentFrame.Navigating -= OnFrameNavigating;
                this.ContentFrame.Navigated -= OnFrameNavigated;
            }
            if (this.RootFrame != null)
            {
                this.RootFrame.Navigating -= OnFrameNavigating;
                this.RootFrame.Navigated -= OnFrameNavigated;
            }
            this.NavigateFromCurrentViewModel(null, true);
        }
        #endregion

        #region MethodsPrivate
        private bool NavigateImpl(Type sourcePageType, Type masterPageType, object parameter)
        {
            if (masterPageType == null)
            {
                return this.RootFrame.Navigate(sourcePageType, parameter);
            }

            var masterPage = this.RootFrame.Content as Page;
            if (masterPage == null || masterPage.GetType() != masterPageType)
            {
                this.RootFrame.Navigate(masterPageType, parameter);
                masterPage = this.RootFrame.Content as Page;
            }
            if (this.ContentFrame != null)
            {
                this.ContentFrame.Navigating -= OnFrameNavigating;
                this.ContentFrame.Navigated -= OnFrameNavigated;
            }
            this.ContentFrame = masterPage.FindName("ContentFrame") as Frame;
            this.ContentFrame.Navigating += OnFrameNavigating;
            this.ContentFrame.Navigated += OnFrameNavigated;
            return this.ContentFrame.Navigate(sourcePageType, parameter);
        }

        private void OnFrameNavigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            this.NavigateFromCurrentViewModel(sender, false);
        }
        private void OnFrameNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            Frame frame = sender as Frame;
            if (frame != null)
            {
                var newView = frame.Content as FrameworkElement;
                if (newView != null)
                {
                    var newViewModel = newView.DataContext as INavigationAware;
                    if (newViewModel != null)
                    {
                        newViewModel.OnNavigatedTo(e.Parameter, e.NavigationMode);
                    }
                }
            }
        }
         /// <summary>
        /// Navigates away from the current viewmodel.
        /// </summary>
        /// <param name="suspending">True if it is navigating away from the viewmodel due to a suspend event.</param>
        private void NavigateFromCurrentViewModel(object sender, bool suspending)
        {
            var frame = sender as Frame;
            if (frame != null)
            {
                var oldView = frame.Content as FrameworkElement;
                if (oldView != null)
                {
                    var oldViewModel = oldView.DataContext as INavigationAware;
                    if (oldViewModel != null)
                    {
                        oldViewModel.OnNavigatedFrom(suspending);
                    }
                }
            }
        }
        #endregion
    }
}
