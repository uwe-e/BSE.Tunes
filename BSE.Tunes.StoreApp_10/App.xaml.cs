using Windows.UI.Xaml;
using System.Threading.Tasks;
using BSE.Tunes.StoreApp.Services;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Microsoft.Practices.ServiceLocation;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.IO;
using Windows.UI.ViewManagement;

namespace BSE.Tunes.StoreApp
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper
    {
        #region FieldsPrivate
        private SettingsService m_settingsService;
        #endregion
        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region app settings

            // some settings must be set in app.constructor
            m_settingsService = SettingsService.Instance;
            RequestedTheme = m_settingsService.UseLightTheme ? ApplicationTheme.Light : ApplicationTheme.Dark;
            CacheMaxDuration = m_settingsService.CacheMaxDuration;
            AutoSuspendAllFrames = true;
            AutoRestoreAfterTerminated = true;
            AutoExtendExecutionSession = true;

            #endregion
        }

        public override UIElement CreateRootElement(IActivatedEventArgs e)
        {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
            //The style resource contains the mediaelement and registers the player service
            service.Frame.Style = Resources["RootFrameStyle"] as Style;
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = (Windows.UI.Color)Current.Resources["SystemAltHighColor"];
                    titleBar.ButtonForegroundColor = (Windows.UI.Color)Current.Resources["SystemBaseHighColor"];
                    titleBar.ButtonHoverBackgroundColor = (Windows.UI.Color)Current.Resources["SystemChromeMediumLowColor"];
                    titleBar.ButtonHoverForegroundColor = (Windows.UI.Color)Current.Resources["SystemAccentColor"];
                    titleBar.BackgroundColor = (Windows.UI.Color)Current.Resources["SystemAltHighColor"];
                    titleBar.ForegroundColor = (Windows.UI.Color)Current.Resources["SystemBaseHighColor"];
                }
            }
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundColor = (Windows.UI.Color)Current.Resources["SystemChromeLowColor"];
                    statusBar.BackgroundOpacity = 1;
                    statusBar.ForegroundColor = (Windows.UI.Color)Current.Resources["SystemBaseHighColor"];
                }
            }
            IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();
            Task<bool> isAccessibleTask = Task.Run(async () => await dataService.IsHostAccessible());
            try
            {
                isAccessibleTask.Wait();
                bool isAccessible = isAccessibleTask.Result;
                if (isAccessible)
                {
                    IAuthenticationService authenticationHandler = ServiceLocator.Current.GetInstance<IAuthenticationService>();
                    Task<User> verifyUserTask = Task.Run(async () => await authenticationHandler.VerifyUserAuthenticationAsync());
                    try
                    {
                        verifyUserTask.Wait();
                        User user = verifyUserTask.Result;
                        if (user != null)
                        {
                            //Deletes the tmp download folder with its files from the local store.
                            await LocalStorage.ClearTempFolderAsync();

                            m_settingsService.IsFullScreen = false;
                            await NavigationService.NavigateAsync(typeof(Views.MainPage));
                        }
                        else
                        {
                            m_settingsService.IsFullScreen = true;
                            await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage));
                        }
                    }
                    catch (Exception exception)
                    {
                        m_settingsService.IsFullScreen = true;
                        await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage), exception);
                    }
                }
            }
            catch (Exception)
            {
                m_settingsService.IsFullScreen = true;
                await NavigationService.NavigateAsync(typeof(Views.ServiceUrlWizzardPage));
            }
        }
    }
}

