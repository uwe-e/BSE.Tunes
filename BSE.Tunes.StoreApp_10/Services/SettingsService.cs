using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace BSE.Tunes.StoreApp.Services
{
    public class SettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();
        Template10.Services.SettingsService.ISettingsHelper m_settingsHelper;
        private SettingsService()
        {
            m_settingsHelper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public bool UseShellBackButton
        {
            get
            {
                return m_settingsHelper.Read<bool>(nameof(UseShellBackButton), true);
            }
            set
            {
                m_settingsHelper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.GetDispatcherWrapper().Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                });
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = m_settingsHelper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                m_settingsHelper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Views.Shell.HamburgerMenu.RefreshStyles(value, true);
            }
        }

        public TimeSpan CacheMaxDuration
        {
            get
            {
                return m_settingsHelper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2));
            }
            set
            {
                m_settingsHelper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        public bool ShowHamburgerButton
        {
            get
            {
                return m_settingsHelper.Read<bool>(nameof(ShowHamburgerButton), true);
            }
            set
            {
                m_settingsHelper.Write(nameof(ShowHamburgerButton), value);
                Views.Shell.HamburgerMenu.HamburgerButtonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsFullScreen
        {
            get
            {
                return m_settingsHelper.Read<bool>(nameof(IsFullScreen), false);
            }
            set
            {
                m_settingsHelper.Write(nameof(IsFullScreen), value);
                Views.Shell.HamburgerMenu.IsFullScreen = value;
            }
        }
        /// <summary>
        /// Gets or sets the url that contains the service
        /// </summary>
        public string ServiceUrl
        {
            get
            {
                return m_settingsHelper.Read<string>(nameof(ServiceUrl), null);
            }
            set
            {
                m_settingsHelper.Write(nameof(ServiceUrl), value);
            }
        }
    }
}

