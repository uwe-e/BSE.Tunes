using BSE.Tunes.StoreApp.IO;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Services;
using CommonServiceLocator;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Controls;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

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

            //For localization tests
            //var culture = new System.Globalization.CultureInfo("en-US");
            //var culture = new System.Globalization.CultureInfo("de-DE");
            //Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = culture.Name;
            //System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            //System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

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
            //Relaunching suspended app on W10 Mobile shows blank page.
            //https://github.com/Windows-XAML/Template10/issues/1286
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
            //The style resource contains the mediaelement and registers the player service
            service.Frame.Style = Resources["RootFrameStyle"] as Style;

            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }
        public override Task OnInitializeAsync(IActivatedEventArgs args)
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
            return base.OnInitializeAsync(args);
        }
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();
            try
            {
                var isAccessible = await dataService.IsHostAccessible().ConfigureAwait(true);
                if (isAccessible)
                {
                    try
                    {
                        IAuthenticationService authenticationHandler = ServiceLocator.Current.GetInstance<IAuthenticationService>();
                        User user = await authenticationHandler.VerifyUserAuthenticationAsync().ConfigureAwait(true);
                        if (user != null)
                        {
                            //clears the cache only if the application lauches
                            if (startKind == StartKind.Launch)
                            {
                                //Deletes the tmp download folder with its files from the local store.
                                await LocalStorage.ClearTempFolderAsync();
                                m_settingsService.ApplyStartUpSettings();

                                await NavigationService.NavigateAsync(typeof(Views.MainPage));
                            }
                        }
                        else
                        {
                            m_settingsService.IsFullScreen = true;
                            await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage));
                        }
                    }
                    catch (AggregateException ae)
                    {
                        var unauthorizedAccessException = ae.Flatten().InnerExceptions
                            .Select(exception => exception as UnauthorizedAccessException).FirstOrDefault();
                        if (unauthorizedAccessException != null)
                        {
                            await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage), unauthorizedAccessException);
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

