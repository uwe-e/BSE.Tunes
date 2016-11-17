using System.ComponentModel;
using System.Linq;
using System;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Mvvm;
using BSE.Tunes.StoreApp.Services;

namespace BSE.Tunes.StoreApp.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance
        {
            get; set;
        }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        SettingsService m_settings;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            m_settings = SettingsService.Instance;
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
            HamburgerMenu.RefreshStyles(m_settings.AppTheme, true);
            HamburgerMenu.IsFullScreen = m_settings.IsFullScreen;
            HamburgerMenu.HamburgerButtonVisibility = m_settings.ShowHamburgerButton ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

