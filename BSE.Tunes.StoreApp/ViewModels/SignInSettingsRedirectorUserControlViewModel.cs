using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SignInSettingsRedirectorUserControlViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private INavigationService m_navigationService;
        private ICommand m_redirectCommand;
        #endregion

        #region Properties
        public ICommand RedirectCommand
        {
            get
            {
                return this.m_redirectCommand ??
                    (this.m_redirectCommand = new RelayCommand(this.Redirect));
            }
        }
        #endregion

        #region MethodsPublic
        public SignInSettingsRedirectorUserControlViewModel(INavigationService navigationService)
        {
            this.m_navigationService = navigationService;
        }
        #endregion

        #region MethodsPrivate
        private void Redirect()
        {
            this.m_navigationService.Navigate(typeof(SignInSettingsPage));
        }
        #endregion
    }
}
