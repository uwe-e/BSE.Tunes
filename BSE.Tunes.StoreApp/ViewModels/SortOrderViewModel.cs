using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SortOrderViewModel : ViewModelBase
    {
        #region FieldsPrivate
        #endregion

        public string Name
        {
            get
            {
                return this.SortOrder.Name;
            }
        }

        public SortOrder SortOrder
        {
            get;
            private set;
        }

        public SortOrderViewModel(SortOrder sortOrder)
        {
            this.SortOrder = sortOrder;
        }
    }
}
