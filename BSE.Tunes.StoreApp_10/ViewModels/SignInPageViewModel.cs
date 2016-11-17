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
    public class SignInPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IAuthenticationHandler m_authenticationHandler;
        private IDialogService m_dialogSService;
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
        public bool UseSecureLogin
        {
            get
            {
                return this.m_useSecureLogin;
            }
            set
            {
                this.m_useSecureLogin = value;
                this.RaisePropertyChanged("UseSecureLogin");
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
        public SignInPageViewModel()
        {
            m_authenticationHandler = AuthenticationHandler.Instance;
            m_dialogSService = DialogService.Instance;
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
                User user = await this.m_authenticationHandler.AuthenticateAsync(UserName, Password, UseSecureLogin);
                await NavigationService.NavigateAsync(typeof(Views.MainPage));
            }
            catch (Exception exception)
            {
                await m_dialogSService.ShowAsync(exception.Message, "Fähler");
            }
        }
        #endregion
    }
}
