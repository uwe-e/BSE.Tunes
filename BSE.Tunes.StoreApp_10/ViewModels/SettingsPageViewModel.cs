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
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        #region FieldsPrivate
        SettingsService m_settings;
        private bool m_themeSelectionHasChanged;
        #endregion

        #region Properties
        public SettingsPartViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
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
    
    public class AboutPartViewModel : ViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public Uri RateMe => new Uri("http://aka.ms/template10");
    }
}

