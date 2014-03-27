using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Interfaces;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class HostSettingsPageViewModel : ViewModelBase, INavigationAware
    {
        #region FieldsPrivate
        private IDataService m_dataService;
        private IHostsettingsService m_hostSettingsService;
        private INavigationService m_navigationService;
		private IResourceService m_resourceService;
		private IAccountService m_accountService;
        private RelayCommand m_saveHostCommand;
        private ICommand m_cancelCommand;
        private string m_strServiceUrl;
        private string m_errorMessage;
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
        public RelayCommand SaveHostCommand
        {
            get
            {
                return this.m_saveHostCommand ??
                    (this.m_saveHostCommand = new RelayCommand(this.SaveHost, this.CanExecuteSaveHostCommand));
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
		public HostSettingsPageViewModel(IDataService dataService, IHostsettingsService hostSettingsService, IAccountService accountService, INavigationService navigationService, IResourceService resourceService)
        {
            this.m_dataService = dataService;
			this.m_accountService = accountService;
            this.m_navigationService = navigationService;
            this.m_hostSettingsService = hostSettingsService;
			this.m_resourceService = resourceService;
            this.ServiceUrl = this.m_hostSettingsService.ServiceUrl;
        }
        public void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode)
        {
        }
        public void OnNavigatedFrom(bool suspending)
        {
        }
        #endregion

        #region MethodsPrivate
        private bool CanExecuteSaveHostCommand()
        {
            return !string.IsNullOrEmpty(this.ServiceUrl);
        }
        private void SaveHost()
        {
            this.ErrorMessage = null;
            if (!string.IsNullOrEmpty(this.ServiceUrl))
            {
                this.m_hostSettingsService.SetServiceUrl(this.ServiceUrl);
                Task<bool> isAccessibleTask = Task.Run(async () => await this.m_dataService.IsHostAccessible());
                try
                {
                    isAccessibleTask.Wait();
                    bool isAccessible = isAccessibleTask.Result;
                    if (isAccessible)
                    {
						System.Threading.Tasks.Task<TunesUser> verifyUserTask = System.Threading.Tasks.Task.Run(async () => await this.m_accountService.VerifyUserAuthentication());
						verifyUserTask.Wait();
						TunesUser tunesUser = verifyUserTask.Result;
						if (tunesUser != null)
						{
							this.m_navigationService.Navigate(typeof(MainPage));
						}
						else
						{
							this.m_accountService.ServiceUrl = this.m_hostSettingsService.ServiceUrl;
							this.m_navigationService.Navigate(typeof(SignInSettingsPage));
						}
                    }
                }
                catch (AggregateException ae)
                {
                    string errorMessage = string.Empty;
                    foreach (var e in ae.Flatten().InnerExceptions)
                    {
                        if (e != null && !string.IsNullOrEmpty(e.Message))
                        {
                            errorMessage += e.Message + Environment.NewLine;
                        }
                    }
					ErrorMessage = this.m_resourceService.GetString("HostNotAvailableExceptionMessage", errorMessage);
                }
                catch (Exception exception)
                {
					ErrorMessage = this.m_resourceService.GetString("HostNotAvailableExceptionMessage", exception.Message);
                }
            }
        }
        #endregion
    }
}
