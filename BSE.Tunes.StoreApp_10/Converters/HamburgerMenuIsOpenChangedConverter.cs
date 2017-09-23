using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BSE.Tunes.StoreApp.Converters
{
	public class HamburgerMenuIsOpenChangedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			Template10.Common.ChangedEventArgs<bool> isOpenEventArgs = value as Template10.Common.ChangedEventArgs<bool>;
			return isOpenEventArgs?.NewValue;
		}

		public object ConvertBack(object value, System.Type type, object parameter, string language)
		{
			throw new NotImplementedException(); //doing one-way binding so this is not required.
		}
	}
}
