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
    public class TopAppBarUserControlViewModel : ViewModelBase
    {
        #region FieldsPrivate
        INavigationService m_navigationService;
        ICommand m_homeNavigationCommand;
        #endregion

        #region Properties
        public ICommand HomeNavigationCommand
        {
            get
            {
                return this.m_homeNavigationCommand ??
                    (this.m_homeNavigationCommand = new RelayCommand(()=> this.m_navigationService.Navigate(typeof(MainPage), typeof(MasterPage))));
            }
        }
        #endregion

        #region MethodsPublic
        public TopAppBarUserControlViewModel(INavigationService navigationService)
        {
            this.m_navigationService = navigationService;
        }
        #endregion
    }
}
