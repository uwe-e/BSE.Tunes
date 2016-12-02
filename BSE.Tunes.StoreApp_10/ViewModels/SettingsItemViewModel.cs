using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public abstract class SettingsItemViewModel : ViewModelBase, ISettingsItemViewModel
    {
        #region FieldsPrivate
        private string m_text;
        private string m_description;
        private string m_icon;
        private ICommand m_navigationCommand;
        #endregion

        #region Properties
        public virtual string Title
        {
            get
            {

                return m_text;
            }
            set
            {
                m_text = value;
                RaisePropertyChanged("Text");
            }
        }
        public virtual string Description
        {
            get
            {

                return m_description;
            }
            set
            {

                m_description = value;
                RaisePropertyChanged("Description");
            }
        }
        public virtual string Icon
        {
            get
            {

                return m_icon;
            }
            set
            {

                m_icon = value;
                RaisePropertyChanged("Icon");
            }
        }
        public ICommand NavigationCommand => m_navigationCommand ?? (m_navigationCommand = new RelayCommand<object>(vm => Navigate()));
        #endregion

        #region MethodsPublic
        public SettingsItemViewModel()
        {
            NavigationService = NavigationService ?? Template10.Common.WindowWrapper.Current().NavigationServices.FirstOrDefault();
        }
        public abstract void Navigate();
        #endregion
    }
}
