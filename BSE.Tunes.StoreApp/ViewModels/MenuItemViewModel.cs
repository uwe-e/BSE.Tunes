using BSE.Tunes.StoreApp.Messaging;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class MenuItemViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private string m_strContent;
        #endregion

        #region Properties
        public string Content
        {
            get
            {
                return this.m_strContent;
            }
            set
            {
                this.m_strContent = value;
                RaisePropertyChanged("Content");
            }
        }
        #endregion

        #region MethodsPublic
        public MenuItemViewModel()
        {
        }
        #endregion
    }
}
