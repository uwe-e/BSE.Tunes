using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class GroupUserControlViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private DataGroupViewModel m_dataGroup;
        private bool m_isBusy;
        private ICommand m_selectGroupHeaderCommand;
        #endregion

        #region Properties
        public DataGroupViewModel DataGroup
        {
            get
            {
                return this.m_dataGroup;
            }
            set
            {
                this.m_dataGroup = value;
                RaisePropertyChanged("DataGroup");
            }
        }
        public bool IsBusy
        {
            get
            {
                return this.m_isBusy;
            }
            set
            {
                this.m_isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }
        public ICommand SelectGroupHeaderCommand
        {
            get
            {
                return this.m_selectGroupHeaderCommand ??
                    (this.m_selectGroupHeaderCommand = new RelayCommand(this.OnSelectGroupHeader));
            }
        }
        #endregion

        #region MethodsPublic
        public virtual void OnSelectGroupHeader()
        {
        }
        #endregion
    }
}
