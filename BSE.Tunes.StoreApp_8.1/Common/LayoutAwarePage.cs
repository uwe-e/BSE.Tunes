using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Common
{
	/// <summary>
	/// In Windows 8, Microsoft Visual Studio templates define the LayoutAwarePage class to manage the visual states based on
	/// the ApplicationViewState. In Windows 8.1, ApplicationViewState is deprecated and LayoutAwarePage is no longer included in
	/// the Visual Studio templates for Windows Store apps. Continuing to use the LayoutAwarePage code can break your app. To fix
	/// this, rewrite your view to accommodate the new minimum resolution, and create events based on the window size. 
	/// </summary>
	public class LayoutAwarePage : Page
	{
		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.  The Parameter
		/// property provides the group to be displayed.</param>
		protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			this.Loaded += (sender, args) =>
			{
				Window.Current.SizeChanged += Window_SizeChanged;
			};
			this.Unloaded += (sender, args) =>
			{
				Window.Current.SizeChanged -= Window_SizeChanged;
			};
		}

		void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
		{
			if (e.Size.Width <= 500)
			{
				//VisualStateManager.GoToState(this, state.State, transitions);
			}
			else if (e.Size.Height > e.Size.Width)
			{
				//VisualStateManager.GoToState(this, state.State, transitions);
			}
			else
			{
				//VisualStateManager.GoToState(this, state.State, transitions);
			}
		}
	}
}
