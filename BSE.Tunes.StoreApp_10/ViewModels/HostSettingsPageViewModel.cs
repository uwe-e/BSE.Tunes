using BSE.Tunes.StoreApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class HostSettingsPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private SettingsService m_settingsService;
        private IDialogService m_dialogSService;
        private IDataService m_dataService;
        private IAuthenticationHandler m_authenticationHandler;
        private RelayCommand m_saveHostCommand;
        private string m_strServiceUrl;
        #endregion

        #region Properties
        public string ServiceUrl
        {
            get
            {
                return this.m_strServiceUrl;
            }
            set
            {
                this.m_strServiceUrl = value;
                this.RaisePropertyChanged("ServiceUrl");
                this.SaveHostCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region MethodsPublic
        public HostSettingsPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                m_settingsService = SettingsService.Instance;
                m_dialogSService = DialogService.Instance;
                m_dataService = DataService.Instance;
                m_authenticationHandler = AuthenticationHandler.Instance;
            }
        }
        public RelayCommand SaveHostCommand => m_saveHostCommand ?? (m_saveHostCommand = new RelayCommand(SaveHost, CanExecuteSaveHostCommand));
        #endregion

        #region MethodsPrivate
        private bool CanExecuteSaveHostCommand()
        {
            return !string.IsNullOrEmpty(this.ServiceUrl);
        }
        private async void SaveHost()
        {
            m_settingsService.ServiceUrl = ServiceUrl;
            try
            {
                bool isAccessible = await this.m_dataService.IsHostAccessible();
                if (isAccessible)
                {
                    //try
                    //{

                    //}
                }
            }
            catch (AggregateException ae)
            {
            }
                await m_dialogSService.ShowAsync("test", "title");
            //Template10.Services. DialogService
        }
        #endregion
    }
}
