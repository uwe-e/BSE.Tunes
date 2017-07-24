using BSE.Tunes.StoreApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using BSE.Tunes.StoreApp.Models;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ServiceUrlWizzardPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private SettingsService m_settingsService;
        private IDialogService m_dialogSService;
        private IAuthenticationService m_authenticationHandler;
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
        public ServiceUrlWizzardPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                m_settingsService = SettingsService.Instance;
                m_dialogSService = DialogService.Instance;
                m_authenticationHandler = AuthenticationService.Instance;
                ServiceUrl = m_settingsService.ServiceUrl;
            }
        }
        public RelayCommand SaveHostCommand => m_saveHostCommand ?? (m_saveHostCommand = new RelayCommand(SaveUrl));
        #endregion

        #region MethodsPrivate
        public async void SaveUrl()
        {
            var serviceUrl = ServiceUrl;
            try
            {
                if (!string.IsNullOrEmpty(serviceUrl))
                {
                    UriBuilder uriBuilder = new UriBuilder(serviceUrl);
                    serviceUrl = uriBuilder.Uri.AbsoluteUri;
                }

                await DataService.IsHostAccessible(serviceUrl);
                m_settingsService.ServiceUrl = serviceUrl;
                try
                {
                    User user = await m_authenticationHandler.VerifyUserAuthenticationAsync().ConfigureAwait(true);
                    if (user == null)
                    {
                        m_settingsService.IsFullScreen = true;
                        await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage));
                    }
                    else
                    {
                        m_settingsService.IsFullScreen = false;
                        await NavigationService.NavigateAsync(typeof(Views.MainPage));
                    }
                }
                catch(UnauthorizedAccessException)
                {
                    m_settingsService.IsFullScreen = true;
                    await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage));
                }
            }
            catch (Exception)
            {
                await m_dialogSService.ShowMessageDialogAsync(
                    ResourceService.GetString("ServiceUrlNotAvailableExceptionMessage", "The address of your webserver was entered incorrectly or the webserver is not available."),
                    ResourceService.GetString("ExceptionMessageDialogHeader", "Error"));
            }
        }
        #endregion
    }
}
