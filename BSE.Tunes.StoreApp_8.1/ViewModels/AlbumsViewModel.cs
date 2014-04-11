using BSE.Tunes.Data;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;
using BSE.Tunes.StoreApp.Services;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class AlbumViewModel : ViewModelBase
    {
        #region FieldsPrivate
		private IDataService m_dataService;
		#endregion

        #region Properties
        public string Title
        {
            get
            {
                return this.Album.Title;
            }
        }

        public Artist Artist
        {
            get
            {
                return this.Album.Artist;
            }
        }
		public Uri CoverSource
		{
			get
			{
				return this.m_dataService != null ? this.m_dataService.GetImage(this.Album.AlbumId, true) : null;
			}
		}
        public Genre Genre
        {
            get
            {
                return this.Album.Genre;
            }
        }
        public int? Year
        {
            get
            {
                return this.Album.Year;
            }
        }
        public Track[] Tracks
        {
            get
            {
                return this.Album.Tracks;
            }
        }
        public Album Album
        {
            get;
            private set;
        }
        #endregion

        #region MethodsPublic
        public AlbumViewModel(IDataService dataService, Album album)
        {
			this.m_dataService = dataService;
			this.Album = album;
        }
        #endregion
    }
}
