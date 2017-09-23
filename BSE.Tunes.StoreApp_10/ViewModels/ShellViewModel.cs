using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
	public class ShellViewModel : SearchSuggestionsViewModel
	{
		#region FieldsPrivate
		private SettingsService m_settingsService;
		private bool m_isHamburgerMenuOpen;
		private ICommand m_isOpenChangedCommand;
		private ICommand m_openMenuPanelCommand;
		#endregion

		#region Properties
		public bool IsHamburgerMenuOpen
		{
			get
			{
				return m_isHamburgerMenuOpen;
			}
			set
			{
				m_isHamburgerMenuOpen = value;
				RaisePropertyChanged(() => IsHamburgerMenuOpen);
			}
		}
		public ICommand OpenMenuPanelCommand => m_openMenuPanelCommand ?? (m_openMenuPanelCommand = new RelayCommand(() =>
		{
			IsHamburgerMenuOpen = true;
		}));

		public ICommand IsOpenChangedCommand => m_isOpenChangedCommand ?? (m_isOpenChangedCommand = new RelayCommand<bool> ((isOpen) =>
		{
			m_settingsService.IsHamburgerMenuOpen = isOpen;
		}));
		#endregion

		#region MethodsPublic
		public ShellViewModel()
		{
			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				m_settingsService = SettingsService.Instance;
			}
		}
		#endregion
	}
}
