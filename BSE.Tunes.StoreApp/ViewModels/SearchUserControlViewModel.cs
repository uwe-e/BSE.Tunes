using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{ 
    public class SearchUserControlViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private ISearchPaneService m_searchPaneService;
        private ICommand m_searchCommand;
        #endregion

        #region Properties
        public ICommand SearchCommand
        {
            get
            {
                return this.m_searchCommand ??
                    (this.m_searchCommand = new RelayCommand(this.ShowSearchPane));
            }
        }
        #endregion

        #region MethodsPublic
        public SearchUserControlViewModel(ISearchPaneService searchPaneService)
        {
            this.m_searchPaneService = searchPaneService;
        }
        #endregion

        #region MethodsPrivate
        private void ShowSearchPane()
        {
            this.m_searchPaneService.Show();
        }
        #endregion
    }
}
