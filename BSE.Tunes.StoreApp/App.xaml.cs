using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.ViewModels;
using BSE.Tunes.StoreApp.Views;
using Callisto.Controls;
using Callisto.Controls.SettingsManagement;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace BSE.Tunes.StoreApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Gets or sets the navigation service.
        /// </summary>
        /// <value>
        /// The navigation service.
        /// </value>
        public INavigationService NavigationService { get; private set; }
        /// <summary>
        /// Gets or sets the dialog service
        /// </summary>
        public IDialogService DialogService { get; private set; }
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            
            //For localization tests
            //var culture = new CultureInfo("en-US");
            //Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = culture.Name;
            //CultureInfo.DefaultThreadCurrentCulture = culture;
            //CultureInfo.DefaultThreadCurrentUICulture = culture;

            this.Suspending += OnSuspending;
        }
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            AppSettings.Current.AddCommand<HostSettingsRedirectorUserControl>(
                ServiceLocator.Current.GetInstance<IResourceService>().GetString("IDS_HostSettingsRedirector_Header", "Host Settings"),
                SettingsFlyout.SettingsFlyoutWidth.Wide);

            AppSettings.Current.AddCommand<SignInSettingsRedirectorUserControl>(
                ServiceLocator.Current.GetInstance<IResourceService>().GetString("IDS_SignInSettingsRedirector_Header", "Host Settings"),
                SettingsFlyout.SettingsFlyoutWidth.Wide);

            AppSettings.Current.AddCommand<PrivacyStatementUserControl>(
                ServiceLocator.Current.GetInstance<IResourceService>().GetString("IDS_PrivacysStatement_Header", "Privacy"),
                SettingsFlyout.SettingsFlyoutWidth.Narrow);

            this.DialogService = ServiceLocator.Current.GetInstance<IDialogService>();

            Frame rootFrame = Window.Current.Content as Frame;
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                //The style resource contains the mediaelement and registers the player service
                rootFrame.Style = Resources["RootFrameStyle"] as Style;
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
                this.NavigationService = new NavigationService(rootFrame);
                GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<INavigationService>(() => this.NavigationService);
            }

            if (rootFrame.Content == null)
            {
                IHostsettingsService hostSettings = ServiceLocator.Current.GetInstance<IHostsettingsService>();
                IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();

                System.Threading.Tasks.Task<bool> isAccessibleTask = System.Threading.Tasks.Task.Run(async () => await dataService.IsHostAccessible());
                try
                {
                    isAccessibleTask.Wait();
                    bool isAccessible = isAccessibleTask.Result;
                    if (isAccessible)
                    {
                        IAccountService accountService = ServiceLocator.Current.GetInstance<IAccountService>();
                        accountService.ServiceUrl = hostSettings.ServiceUrl;
                        System.Threading.Tasks.Task<TunesUser> verifyUserTask = System.Threading.Tasks.Task.Run(async () => await accountService.VerifyUserAuthentication());
                        try
                        {
                            verifyUserTask.Wait();

                            TunesUser tunesUser = verifyUserTask.Result;
                            if (tunesUser != null)
                            {
                                // When the navigation stack isn't restored navigate to the first page,
                                // configuring the new page by passing required information as a navigation
                                // parameter
                                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                                {
                                    throw new Exception("Failed to create initial page");
                                }
                            }
                            else
                            {
                                // When the navigation stack isn't restored navigate to the first page,
                                // configuring the new page by passing required information as a navigation
                                // parameter
                                if (!rootFrame.Navigate(typeof(SignInSettingsPage), args.Arguments))
                                {
                                    throw new Exception("Failed to create initial page");
                                }
                            }
                        }
                        catch (Exception unauthorizedAccessException)
                        {
                            if (!rootFrame.Navigate(typeof(SignInSettingsPage), args.Arguments))
                            {
                                throw new Exception("Failed to create initial page");
                            }
                        }
                    }
                    else
                    {
                        if (!rootFrame.Navigate(typeof(HostSettingsPage), args.Arguments))
                        {
                            throw new Exception("Failed to create initial page");
                        }
                    }
                }
                catch (Exception)
                {
                    if (!rootFrame.Navigate(typeof(HostSettingsPage), args.Arguments))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            this.NavigationService.Suspending();
            deferral.Complete();
        }
        protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            if (args != null && !string.IsNullOrEmpty(args.QueryText))
            {
                this.NavigationService.Navigate(typeof(SearchResultPage), args.QueryText);
            }
        }
    }
}
