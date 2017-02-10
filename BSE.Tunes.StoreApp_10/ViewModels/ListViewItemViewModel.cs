using BSE.Tunes.StoreApp.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ListViewItemViewModel : GalaSoft.MvvmLight.ViewModelBase, IPlaceableContext
    {
        #region FieldsPrivate
        private object m_data;
        private bool m_isOpen;
        private Point m_offsetPoint;
        #endregion

        #region Properties
        public bool IsOpen
        {
            get
            {
                return m_isOpen;
            }
            set
            {
                this.m_isOpen = value;
                RaisePropertyChanged("IsOpen");
            }
        }
        public Point OffsetPoint
        {
            get
            {
                return m_offsetPoint;
            }
            set
            {
                this.m_offsetPoint = value;
                RaisePropertyChanged("OffsetPoint");
            }
        }
        public object Data
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
                RaisePropertyChanged("Data");
            }
        }
        #endregion
    }
}
