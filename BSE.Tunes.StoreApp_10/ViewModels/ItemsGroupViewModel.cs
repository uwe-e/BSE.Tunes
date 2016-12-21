using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ItemsGroupViewModel : ItemViewModel
    {
        #region FieldsPrivate
        private ObservableCollection<ItemViewModel> m_items;
        #endregion

        #region Properties
        public ObservableCollection<ItemViewModel> Items
        {
            get
            {
                return this.m_items;
            }
        }
        #endregion

        #region MethodsPublic
        public ItemsGroupViewModel()
        {
            this.m_items = new ObservableCollection<ItemViewModel>();
        }
        #endregion
    }
}
