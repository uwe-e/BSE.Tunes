using BSE.Tunes.StoreApp.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class GridPanelItemViewModel : ListViewItemViewModel
    {
        #region FieldsPrivate
        private string m_title;
        private string m_subTitle;
        private string m_description;
		private Uri m_imageSource;
        private BitmapSource m_bitmapSource;
        #endregion

        #region Properties
        public string Title
        {
            get
            {
                return this.m_title;
            }
            set
            {
                this.m_title = value;
                RaisePropertyChanged("Title");
            }
        }
        public string Subtitle
        {
            get
            {
                return this.m_subTitle;
            }
            set
            {
                this.m_subTitle = value;
                RaisePropertyChanged("Subtitle");
            }
        }
        public string Description
        {
            get
            {
                return this.m_description;
            }
            set
            {
                this.m_description = value;
                RaisePropertyChanged("Description");
            }
        }
		public Uri ImageSource
		{
			get
			{
				return this.m_imageSource;
			}
			set
			{
				this.m_imageSource = value;
				RaisePropertyChanged("ImageSource");
			}
		}
        public BitmapSource BitmapSource
        {
            get
            {
                return this.m_bitmapSource;
            }
            set
            {
                this.m_bitmapSource = value;
                RaisePropertyChanged("BitmapSource");
            }
        }
        #endregion
    }
}
