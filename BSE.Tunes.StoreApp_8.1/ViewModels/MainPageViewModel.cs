using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.DataModel;
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
    public class MainPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IDataService m_dataService;
		private Uri m_coverSource;
        #endregion

        #region Properties
		public Uri CoverSource
		{
			get
			{
				return this.m_coverSource;
			}
			set
			{
				this.m_coverSource = value;
				RaisePropertyChanged("CoverSource");
			}
		}
        #endregion

        #region MethodsPublic
        public MainPageViewModel(IDataService dataService)
        {
            this.m_dataService = dataService;
			GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<GalaSoft.MvvmLight.Messaging.PropertyChangedMessage<Uri>>(this, true, action =>
			{
				this.CoverSource = action.NewValue;
			});
        }
        #endregion
    }
}
