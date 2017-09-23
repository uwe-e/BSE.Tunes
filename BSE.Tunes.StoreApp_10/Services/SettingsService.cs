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
		#region FieldsPrivate
		private Template10.Services.SettingsService.ISettingsHelper m_settingsHelper;
		private bool m_isStartUp;
		#endregion
		
		#region Properties
		public static SettingsService Instance { get; } = new SettingsService();
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
		/// Gets or sets an information that indicates the HamburgerMenu IsOpen state.
		/// </summary>
		public bool IsHamburgerMenuOpen
		{
			get
			{
				return m_settingsHelper.Read<bool>(nameof(IsHamburgerMenuOpen), true);
			}
			set
			{
				if (m_isStartUp)
				{
					m_settingsHelper.Write(nameof(IsHamburgerMenuOpen), value);
				}
				m_isStartUp = true;
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
		#endregion

		#region MethodsPublic
		public void ApplyStartUpSettings()
		{
			//A successful startup needs no fullscreen. A fullscreen has no visible hamburger menu.
			IsFullScreen = false;
			//Sets the HamurgerMenu's IsOpen state.
			Views.Shell.HamburgerMenu.IsOpen = IsHamburgerMenuOpen;
		}
		#endregion

		#region MethodsPrivate
		private SettingsService()
		{
			m_settingsHelper = new Template10.Services.SettingsService.SettingsHelper();
		}
		#endregion
	}
}

