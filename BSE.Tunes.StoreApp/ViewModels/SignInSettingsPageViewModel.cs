using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Security.Credentials;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SignInSettingsPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IDataService m_dataService;
        private INavigationService m_navigationService;
        private IAccountService m_accoutService;
        private RelayCommand m_saveCommand;
        private ICommand m_cancelCommand;
        private string m_strUserName;
        private string m_strPassword;
        private string m_errorMessage;
        #endregion

        #region Properties
        public string UserName
        {
            get
            {
                return this.m_strUserName;
            }
            set
            {
                this.m_strUserName = value;
                this.SaveCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("UserName");
            }
        }
        public string Password
        {
            get
            {
                return this.m_strPassword;
            }
            set
            {
                this.m_strPassword = value;
                this.SaveCommand.RaiseCanExecuteChanged();
                this.RaisePropertyChanged("Password");
            }
        }
        public string ErrorMessage
        {
            get
            {
                return this.m_errorMessage;
            }
            set
            {
                this.m_errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }
        public RelayCommand SaveCommand
        {
            get
            {
                return this.m_saveCommand ??
                    (this.m_saveCommand = new RelayCommand(this.Save, this.CanExecuteSaveCommand));
            }
        }
        public ICommand CancelCommand
        {
            get
            {
                return this.m_cancelCommand ??
                    (this.m_cancelCommand = new RelayCommand(() =>
                        this.m_navigationService.GoBack()));
            }
        }
        #endregion

        #region MethodsPublic
        public SignInSettingsPageViewModel(IDataService dataService, INavigationService navigationService, IAccountService accountService)
        {
            this.m_dataService = dataService;
            this.m_navigationService = navigationService;
            this.m_accoutService = accountService;
            
            this.LoadData();
        }
        #endregion

        #region MethodsPrivate
        private async void LoadData()
        {
            try
            {
                TunesUser tunesUser = await this.m_accoutService.VerifyUserCredentials();
                if (tunesUser != null)
                {
                    this.UserName = tunesUser.UserName;
                    this.Password = tunesUser.Password;
                }
            }
            catch { }
        }
        private bool CanExecuteSaveCommand()
        {
            return !string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.Password);
        }
        private async void Save()
        {
            this.ErrorMessage = null;
            if (!string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.Password))
            {
                try
                {
                    TunesUser tunesUser = await this.m_accoutService.SignInUser(this.UserName, this.Password);
                    this.m_navigationService.Navigate(typeof(MainPage));
                }
                catch (Exception exception)
                {
                    ErrorMessage = exception.Message;
                }
            }
        }
        #endregion
    }
}
