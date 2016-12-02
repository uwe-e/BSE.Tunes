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
    public class SignOutSettingsPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IAuthenticationService m_authenticationHandler;
        private IDialogService m_dialogSService;
        private SettingsService m_settingsService;
        private RelayCommand m_signOutCommand;
        private string m_userName;
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
                this.SignOutCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("UserName");
            }
        }
        public RelayCommand SignOutCommand
        {
            get
            {
                return this.m_signOutCommand ??
                    (this.m_signOutCommand = new RelayCommand(this.SignOut, this.CanExecuteSignOutCommand));
            }
        }
        #endregion

        #region MethodsPublic
        public SignOutSettingsPageViewModel()
        {
            m_authenticationHandler = AuthenticationService.Instance;
            m_dialogSService = DialogService.Instance;
            m_settingsService = SettingsService.Instance;
            UserName = m_settingsService.User?.UserName; 
        }
        #endregion


        #region MethodsPrivate
        private bool CanExecuteSignOutCommand()
        {
            return !string.IsNullOrEmpty(UserName);
        }
        private async void SignOut()
        {
            try
            {
                await this.m_authenticationHandler.LogoutAsync();
                m_settingsService.IsFullScreen = true;
                await NavigationService.NavigateAsync(typeof(Views.SignInWizzardPage));
            }
            catch (Exception exception)
            {
                await m_dialogSService.ShowAsync(exception.Message, "Fähler");
            }
        }
        #endregion
    }
}
