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

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class AlbumViewModel : ViewModelBase
    {
        #region FieldsPrivate
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

        public byte[] Cover
        {
            get
            {
                return this.Album.Cover;
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
        public AlbumViewModel(Album album)
        {
            this.Album = album;
        }
        #endregion
    }
}
