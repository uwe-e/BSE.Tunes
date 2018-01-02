using BSE.Tunes.StoreApp.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class MenuFlyoutItemViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        #region Events
        public event EventHandler<EventArgs> ItemClicked;
        #endregion

        #region FieldsPrivate
        private string m_text;
        private bool m_isSeparator;
        private ICommand m_menuItemClickedCommand;
        private InsertMode m_insertMode;
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
        public InsertMode InsertMode
        {
            get
            {
                return m_insertMode;
            }
            set
            {
                m_insertMode = value;
                RaisePropertyChanged("InsertMode");
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
		public dynamic Data
		{
			get;
			set;
		}
		public ICommand MenuItemClickedCommand => m_menuItemClickedCommand ?? (m_menuItemClickedCommand = new RelayCommand<MenuFlyoutItemViewModel>(MenuItemClicked));
        #endregion

        #region MethodsPrivate
        private void MenuItemClicked(MenuFlyoutItemViewModel obj)
        {
            if (ItemClicked != null)
            {
                ItemClicked(obj, EventArgs.Empty);
            }
        }
        #endregion
    }
}
