using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class DataGroupViewModel : DataItemViewModel
    {
        #region FieldsPrivate
        private ObservableCollection<DataItemViewModel> m_items;
        #endregion

        #region Properties
        public ObservableCollection<DataItemViewModel> Items
        {
            get
            {
                return this.m_items;
            }
        }
        #endregion

        #region MethodsPublic
        public DataGroupViewModel()
        {
            this.m_items = new ObservableCollection<DataItemViewModel>();
        }
        #endregion
    }
}
