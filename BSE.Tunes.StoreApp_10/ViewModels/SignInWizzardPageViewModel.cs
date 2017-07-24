using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SignInWizzardPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IAuthenticationService m_authenticationService;
        private IDialogService m_dialogSService;
        private SettingsService m_settingsService;
        private RelayCommand m_authenticateCommand;
        private string m_userName;
        private string m_password;
        private bool m_useSecureLogin;
        #endregion

        #region Properties
        public string UserName
        {
            get
            {
                return this.m_userName;
            }
            set
            {
                this.m_userName = value;
                this.AuthenticateCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("UserName");
            }
        }
        public string Password
        {
            get
            {
                return this.m_password;
            }
            set
            {
                this.m_password = value;
                this.AuthenticateCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("Password");
            }
        }
        public RelayCommand AuthenticateCommand
        {
            get
            {
                return this.m_authenticateCommand ??
                    (this.m_authenticateCommand = new RelayCommand(this.Authenticate, this.CanExecuteAuthenticateCommand));
            }
        }
        #endregion

        #region MethodsPublic
        public SignInWizzardPageViewModel()
        {
            m_authenticationService = AuthenticationService.Instance;
            m_dialogSService = DialogService.Instance;
            m_settingsService = SettingsService.Instance;
            UserName = m_settingsService.User?.UserName; 
        }
        #endregion

        #region MethodsPrivate
        private bool CanExecuteAuthenticateCommand()
        {
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
        }
        private async void Authenticate()
        {
            try
            {
                await this.m_authenticationService.AuthenticateAsync(UserName, Password).ConfigureAwait(true);
                m_settingsService.IsFullScreen = false;
                //Clears the cache with the back stack before navigate
                NavigationService.ClearCache(true);
                await NavigationService.NavigateAsync(typeof(Views.MainPage));
                
            }
            catch (Exception exception)
            {
                await m_dialogSService.ShowMessageDialogAsync(exception.Message, ResourceService.GetString("ExceptionMessageDialogHeader", "Error"));
            }
        }
        #endregion
    }
}
