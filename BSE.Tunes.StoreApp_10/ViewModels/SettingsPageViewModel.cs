using System;
using System.Linq;
using System.Threading.Tasks;
using BSE.Tunes.StoreApp.Mvvm;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using BSE.Tunes.StoreApp.Services;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        SettingsService m_settings;
        private bool m_themeSelectionHasChanged;
        #endregion

        #region Properties
        public SettingsPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                m_settings = SettingsService.Instance;
            }
        }

        public bool IsFullScreen
        {
            get
            {
                return m_settings.IsFullScreen;
            }
            set
            {
                m_settings.IsFullScreen = value;
                base.RaisePropertyChanged();
            }
        }

        public bool ThemeSelectionHasChanged
        {
            get
            {
                return m_themeSelectionHasChanged;
            }
            set
            {
                m_themeSelectionHasChanged = value;
                base.RaisePropertyChanged();
            }
        }

        public bool UseLightTheme
        {
            get
            {
                return m_settings.UseLightTheme;
            }
            set
            {
                m_settings.UseLightTheme = value;
                ThemeSelectionHasChanged = true;
                base.RaisePropertyChanged();
            }
        }

        public bool UseDarkTheme
        {
            get
            {
                return !UseLightTheme;
            }
            set
            {
                UseLightTheme = !value;
            }
        }
        #endregion
    }
}

