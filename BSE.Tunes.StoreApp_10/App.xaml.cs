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
            RequestedTheme = m_settingsService.AppTheme;
            CacheMaxDuration = m_settingsService.CacheMaxDuration;
            ShowShellBackButton = m_settingsService.UseShellBackButton;
            AutoSuspendAllFrames = true;
            AutoRestoreAfterTerminated = true;
            AutoExtendExecutionSession = true;

            #endregion
        }

        public override UIElement CreateRootElement(IActivatedEventArgs e)
        {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();
            Task<bool> isAccessibleTask = Task.Run(async () => await dataService.IsHostAccessible());
            try
            {
                isAccessibleTask.Wait();
                bool isAccessible = isAccessibleTask.Result;
                if (isAccessible)
                {
                    IAuthenticationHandler authenticationHandler = ServiceLocator.Current.GetInstance<IAuthenticationHandler>();
                    Task<User> verifyUserTask = Task.Run(async () => await authenticationHandler.VerifyUserAuthenticationAsync());
                    try
                    {
                        verifyUserTask.Wait();
                        User user = verifyUserTask.Result;
                        if (user != null)
                        {
                            m_settingsService.IsFullScreen = false;
                            await NavigationService.NavigateAsync(typeof(Views.MainPage));
                        }
                    }
                    catch (Exception exception)
                    {
                        m_settingsService.IsFullScreen = true;
                        await NavigationService.NavigateAsync(typeof(Views.SignInPage), exception);
                    }
                }
            }
            catch (Exception)
            {
                m_settingsService.IsFullScreen = true;
                await NavigationService.NavigateAsync(typeof(Views.HostSettingsPage));
            }
        }
    }
}

