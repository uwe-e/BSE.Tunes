using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class MenuItemViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        #region FieldsPrivate
        private string m_text;
        private bool m_isSeparator;
        #endregion

        #region Properties
        public bool IsSeparator
        {
            get
            {
                return m_isSeparator;
            }
            set
            {
                m_isSeparator = value;
                RaisePropertyChanged("IsSeparator");
            }
        }
        public string Text
        {
            get
            {
                return this.m_text;
            }
            set
            {
                this.m_text = value;
                RaisePropertyChanged("Text");
            }
        }
        #endregion
    }
}
