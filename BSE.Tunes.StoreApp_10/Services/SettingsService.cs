using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using GalaSoft.MvvmLight.Messaging;
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
        public bool UseLightTheme
        {
            get
            {
                return m_settingsHelper.Read<bool>(nameof(UseLightTheme), false);
            }
            set
            {
                m_settingsHelper.Write(nameof(UseLightTheme), value);
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
                Messenger.Default.Send(new ScreenSizeChangedArgs(value));
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
        public User User
        {
            get
            {
                return m_settingsHelper.Read<User>(nameof(User), null);
            }
            set
            {
                m_settingsHelper.Write(nameof(User), value);
            }
        }
    }
}

