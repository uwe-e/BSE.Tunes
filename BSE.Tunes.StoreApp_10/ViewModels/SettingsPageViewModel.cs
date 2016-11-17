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
        SettingsService m_settings;

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

        public bool ShowHamburgerButton
        {
            get
            {
                return m_settings.ShowHamburgerButton;
            }
            set
            {
                m_settings.ShowHamburgerButton = value;
                base.RaisePropertyChanged();
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
                if (value)
                {
                    ShowHamburgerButton = false;
                }
                else
                {
                    ShowHamburgerButton = true;
                }
            }
        }

        public bool UseShellBackButton
        {
            get
            {
                return m_settings.UseShellBackButton;
            }
            set
            {
                m_settings.UseShellBackButton = value;
                base.RaisePropertyChanged();
            }
        }

        public bool UseLightThemeButton
        {
            get
            {
                return m_settings.AppTheme.Equals(ApplicationTheme.Light);
            }
            set
            {
                m_settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark;
                base.RaisePropertyChanged();
            }
        }

        private string _BusyText = "Please wait...";
        public string BusyText
        {
            get
            {
                return _BusyText;
            }
            set
            {
                Set(ref _BusyText, value);
                _ShowBusyCommand.RaiseCanExecuteChanged();
            }
        }

        RelayCommand _ShowBusyCommand;
        public RelayCommand ShowBusyCommand
            => _ShowBusyCommand ?? (_ShowBusyCommand = new RelayCommand(async () =>
            {
                Views.Busy.SetBusy(true, _BusyText);
                await Task.Delay(5000);
                Views.Busy.SetBusy(false);
            }, () => !string.IsNullOrEmpty(BusyText)));
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

