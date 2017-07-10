using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SettingsMainPageViewModel: ViewModelBase
    {
        #region FieldsPrivate
        private ISettingsItemViewModel m_settingsHostItemViewModel;
        #endregion

        #region Properties
        public ObservableCollection<ISettingsItemViewModel> SettingItems { get; } = new ObservableCollection<ISettingsItemViewModel>();

        public ISettingsItemViewModel SelectedItem
        {
            get
            {
                return m_settingsHostItemViewModel;
            }
            set
            {
                m_settingsHostItemViewModel = value;
                RaisePropertyChanged("SelectedItem");
            }
        }
        #endregion

        #region MethodsPublic
        public SettingsMainPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                SettingItems.Add(new ServiceUrlSettingsItemViewModel());
                SettingItems.Add(new SignOutSettingsItemViewModel());
                SettingItems.Add(new SystemSettingsItemViewModel());
                SettingItems.Add(new AboutItemViewModel());
            }
        }
        #endregion
    }
}
