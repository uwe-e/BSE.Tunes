using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Services;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IResourceService, ResourceService>();
            SimpleIoc.Default.Register<IDataService, DataService>();
            SimpleIoc.Default.Register<IPlayerService, PlayerService>();
            SimpleIoc.Default.Register<IPlayerManager, PlayerManager>();
            SimpleIoc.Default.Register<IAuthenticationService, AuthenticationService>();
            SimpleIoc.Default.Register<ICacheableBitmapService, CacheableBitmapService>();

            //SimpleIoc.Default.Register<SettingsMainPageViewModel>();
        }

        //public SettingsMainPageViewModel SettingsHostPageViewModel
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<SettingsMainPageViewModel>();
        //    }
        //}

        //private MainPageViewModel m_MainPageViewModel;
        //public MainPageViewModel MainPageViewModel => m_MainPageViewModel ?? (m_MainPageViewModel = new MainPageViewModel());

        //private SettingsHostPageViewModel m_settingsHostPageViewModel;
        //public SettingsHostPageViewModel SettingsHostItemViewModel => m_settingsHostPageViewModel ?? (m_settingsHostPageViewModel = new SettingsHostPageViewModel());

        //private ServiceUrlWizzardPageViewModel m_HostSettingsPageViewModel;
        //public ServiceUrlWizzardPageViewModel HostSettingsPageViewModel => m_HostSettingsPageViewModel ?? (m_HostSettingsPageViewModel = new ServiceUrlWizzardPageViewModel());

        private PlayerBarUserControlViewModel m_playerBarViewModel;
        public PlayerBarUserControlViewModel PlayerBarViewModel => m_playerBarViewModel ?? (m_playerBarViewModel = new PlayerBarUserControlViewModel());

    }
}
