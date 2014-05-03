using BSE.Tunes.Data;
using BSE.Tunes.Data.Audio;
using BSE.Tunes.StoreApp.Interfaces;
using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Messaging;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class AlbumDetailPageViewModel : BasePlaylistableViewModel, INavigationAware
    {
        #region FieldsPrivate
        private INavigationService m_navigationService;
        private Album m_album;
		private Uri m_coverSource;
        private PlayerManager m_playerManager;
        private RelayCommand m_playAlbumCommand;
        private RelayCommand m_removeSelectionCommand;
        private ICommand m_goBackCommand;
        private ObservableCollection<Track> m_selectedTracks;
        private bool m_hasSelectedTracks;
        #endregion

        #region Properties
        public ICommand GoBackCommand
        {
            get
            {
                return this.m_goBackCommand ??
                    (this.m_goBackCommand = new RelayCommand(this.m_navigationService.GoBack));
            }
        }
        public Album Album
        {
            get
            {
                return this.m_album;
            }
            set
            {
                this.m_album = value;
                RaisePropertyChanged("Album");
            }
        }
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
        public ObservableCollection<Track> SelectedTracks
        {
            get
            {
                return this.m_selectedTracks;
            }
            set
            {
                this.m_selectedTracks = value;
                RaisePropertyChanged("SelectedTracks");
            }
        }
        public bool HasSelectedTracks
        {
            get
            {
                return this.m_hasSelectedTracks;
            }
            set
            {
                this.m_hasSelectedTracks = value;
                RaisePropertyChanged("HasSelectedTracks");
            }
        }
        public RelayCommand PlayAlbumCommand
        {
            get
            {
                return this.m_playAlbumCommand ??
                    (this.m_playAlbumCommand = new RelayCommand(PlayAlbum, CanExecutePlayAlbum));
            }
        }
        public RelayCommand RemoveSelectionCommand
        {
            get
            {
                return this.m_removeSelectionCommand ??
                    (this.m_removeSelectionCommand = new RelayCommand(RemoveSelection, CanExecuteRemoveSelection));
            }
        }
        public override ObservableCollection<MenuItemViewModel> MenuItemsPlaylist
        {
            get;
            set;
        }
        #endregion

        #region MethodsPublic
        public AlbumDetailPageViewModel(IDataService dataService, IAccountService accountService, INavigationService navigationService, IResourceService resourceService, PlayerManager playerManager, IDialogService dialogService, ICacheableBitmapService cacheableBitmapService)
            : base(dataService, accountService, dialogService, resourceService, cacheableBitmapService)
        {
            this.m_navigationService = navigationService;
            this.m_playerManager = playerManager;
            Messenger.Default.Register<PlaylistChangeMessage>(this, message =>
            {
                this.LoadPlaylists();
            });
        }

        public async void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode)
        {
            if (navigationParameter is int)
            {
                this.Album = await this.DataService.GetAlbumById((int)navigationParameter);
				this.CoverSource = this.DataService.GetImage(this.Album.AlbumId);
				this.PlayAlbumCommand.RaiseCanExecuteChanged();
                this.SelectedTracks = new ObservableCollection<Track>();
                this.SelectedTracks.CollectionChanged += OnSelectedTrackCollectionChanged;
                this.RemoveSelectionCommand.RaiseCanExecuteChanged();
                this.MenuItemsPlaylist = new ObservableCollection<MenuItemViewModel>();
                try
                {
                    this.LoadPlaylists();
                }
                catch (Exception exception)
                {
                    this.DialogService.ShowDialog(exception.Message);
                }
            }
        }
        public void OnNavigatedFrom(bool suspending)
        {
            this.Album = null;
            if (this.SelectedTracks != null)
            {
                this.SelectedTracks.CollectionChanged -= OnSelectedTrackCollectionChanged;
                this.SelectedTracks = null;
            }
        }
        #endregion

        #region MethodsProtected
        protected override void CreateMenuItems(ObservableCollection<Playlist> playlists)
        {
            base.CreateMenuItems(playlists);
        }
        protected override void AddTracksToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                var tracks = this.SelectedTracks;
                if (tracks.Count == 0)
                {
                    tracks = new ObservableCollection<Track>(this.Album.Tracks);
                }
                if (tracks != null)
                {
                    foreach (var track in tracks)
                    {
                        if (track != null)
                        {
                            playlist.Entries.Add(new PlaylistEntry
                            {
                                PlaylistId = playlist.Id,
                                TrackId = track.Id,
                                Guid = Guid.NewGuid()
                            });
                        }
                    }
                }
                base.AddTracksToPlaylist(playlist);
            }
        }
        #endregion

        #region MethodsPrivate
        private bool CanExecutePlayAlbum()
        {
            return this.Album != null && this.Album.Tracks != null && this.Album.Tracks.Count() > 0;
        }
        private void PlayAlbum()
        {
            var tracks = new System.Collections.ObjectModel.ObservableCollection<Track>(this.Album.Tracks);
            var selectedTracks = new System.Collections.ObjectModel.ObservableCollection<Track>(this.SelectedTracks);
            if (selectedTracks != null && selectedTracks.Count() > 0)
            {
                tracks = selectedTracks;
            }
            if (tracks != null)
            {
				var trackIds = tracks.Select(track => track.Id);
				if (trackIds != null)
				{
					this.m_playerManager.PlayTracks(
						new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
						PlayerMode.CD);
				}
				//this.m_playerManager.PlayTracks(tracks, PlayerMode.CD);
				
            }
        }
        private bool CanExecuteRemoveSelection()
        {
            return this.HasSelectedTracks = this.SelectedTracks != null && this.SelectedTracks.Count > 0;
        }
        private void RemoveSelection()
        {
            if (this.SelectedTracks != null)
            {
                try
                {
                    this.SelectedTracks.Clear();
                }
                catch (Exception exception)
                {
                    this.DialogService.ShowDialog(exception.Message);
                }
            }
        }
        private void OnSelectedTrackCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.RemoveSelectionCommand.RaiseCanExecuteChanged();
        }
        
        #endregion
    }
}
