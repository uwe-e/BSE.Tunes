using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IDataService m_dataService;
        #endregion
        #region MethodsPublic
        public HistoryViewModel(IDataService dataService)
        {
            this.m_dataService = dataService;
        }
        public void UpdateHistory(History history)
        {
            this.m_dataService.UpdateHistory(history);
        }
        #endregion
    }
}
