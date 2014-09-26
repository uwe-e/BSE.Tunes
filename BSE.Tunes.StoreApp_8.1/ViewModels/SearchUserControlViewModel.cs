using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{ 
	public class SearchUserControlViewModel : ViewModelBase
	{
		#region FieldsPrivate
		private INavigationService m_navigationService;
		private ICommand m_searchCommand;
		#endregion

		#region Properties
		public ICommand SearchCommand
		{
			get
			{
				return this.m_searchCommand ??
					(this.m_searchCommand = new GalaSoft.MvvmLight.Command.RelayCommand<Windows.UI.Xaml.Controls.SearchBoxQuerySubmittedEventArgs>(this.ShowSearchPane));
			}
		}
		#endregion

		#region MethodsPublic
		public SearchUserControlViewModel(INavigationService navigationService)
		{
			this.m_navigationService = navigationService;
		}
		#endregion

		#region MethodsPrivate
		private void ShowSearchPane(Windows.UI.Xaml.Controls.SearchBoxQuerySubmittedEventArgs args)
		{
			if (args != null && !string.IsNullOrEmpty(args.QueryText))
			{
				this.m_navigationService.Navigate(typeof(BSE.Tunes.StoreApp.Views.SearchResultPage), typeof(MasterPage), args.QueryText);
			}
		}
		#endregion
	}
}
