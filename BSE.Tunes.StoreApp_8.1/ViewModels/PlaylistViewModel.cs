using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Event;
using BSE.Tunes.StoreApp.Interfaces;
using BSE.Tunes.StoreApp.Messaging;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IResourceService m_resourceService;
		private IDataService m_dataService;
		private IAccountService m_accountService;
        private ICacheableBitmapService m_cacheableBitmapService;
		private Playlist m_playlist;
		private int m_playlistId;
        private BitmapSource m_imageSource;
		private string m_numbersOfEntries;
		#endregion

		#region Properties
		public int PlaylistId
		{
			get { return this.m_playlistId; }
			set
			{
				this.m_playlistId = value;
				RaisePropertyChanged("PlaylistId");
			}
		}
		public string NumbersOfEntries
		{
			get
			{
				return this.m_numbersOfEntries;
			}
			set
			{
				this.m_numbersOfEntries = value;
				RaisePropertyChanged("NumbersOfEntries");
			}
		}
		public Playlist Playlist
		{
			get
			{
				return this.m_playlist;
			}
			set
			{
				this.m_playlist = value;
				RaisePropertyChanged("Playlist");
			}
		}
        public BitmapSource ImageSource
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
		#endregion

		#region MethodsPublic
        public PlaylistViewModel(IDataService dataService, IAccountService accountService, IResourceService resourceService, ICacheableBitmapService cacheableBitmapService, int playlistId)
		{
			this.m_dataService = dataService;
			this.m_accountService = accountService;
			this.m_resourceService = resourceService;
            this.m_cacheableBitmapService = cacheableBitmapService;
			this.PlaylistId = playlistId;
			this.LoadData();
			Messenger.Default.Register<PlaylistEntryChangeMessage>(this, message =>
				{
					Playlist changedPlaylist = message.Playlist;
					if (changedPlaylist != null && changedPlaylist.Equals(this.Playlist))
					{
                        this.LoadData();
					}
				});
		}
		#endregion

		#region MethodsPrivate

		private async void LoadData()
		{
			TunesUser user = this.m_accountService.User;
			if (user != null && !string.IsNullOrEmpty(user.UserName))
			{
				this.Playlist = await this.m_dataService.GetPlaylistByIdWithNumberOfEntries(this.PlaylistId, user.UserName);
                if (this.Playlist != null)
                {
                    this.FormatNumberOfEntriesString();

                    System.Collections.ObjectModel.ObservableCollection<Guid> albumIds = await this.m_dataService.GetPlaylistImageIdsById(this.Playlist.Id, user.UserName, 4);
                    if (albumIds != null)
                    {
                        try
                        {
                            this.ImageSource = await this.m_cacheableBitmapService.GetBitmapSource(
                                new ObservableCollection<Uri>(albumIds.Select(id => this.m_dataService.GetImage(id, true))),
                                this.Playlist.Guid.ToString(),
                                160, true);
                        }
                        catch { };
                    }
                }
			}
		}
		private void FormatNumberOfEntriesString()
		{
			int numberOfEntries = 0;
			if (this.Playlist != null)
			{
				numberOfEntries = this.Playlist.NumberEntries;
			}
			this.NumbersOfEntries = string.Format(CultureInfo.CurrentUICulture, "{0} {1}", numberOfEntries, this.m_resourceService.GetString("IDS_PlaylistItem_PartNumberOfEntries", "Songs"));
		}
		#endregion
	}
}
