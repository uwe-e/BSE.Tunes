using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //SimpleIoc.Default.Register<IHostsettingsService, HostsettingsService>();
            SimpleIoc.Default.Register<IAuthenticationHandler, AuthenticationHandler>();
        }

        private MainPageViewModel m_MainPageViewModel;
        public MainPageViewModel MainPageViewModel => m_MainPageViewModel ?? (m_MainPageViewModel = new MainPageViewModel());

        private HostSettingsPageViewModel m_HostSettingsPageViewModel;
        public HostSettingsPageViewModel HostSettingsPageViewModel => m_HostSettingsPageViewModel ?? (m_HostSettingsPageViewModel = new HostSettingsPageViewModel());

    }
}
