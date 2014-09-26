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
    public class AlbumDetailPageViewModel : BaseTracklistPageViewModel, INavigationAware
    {
        #region FieldsPrivate
        private Album m_album;
        private Uri m_coverSource;
        private PlayerManager m_playerManager;
        private RelayCommand m_playAlbumCommand;
        #endregion

        #region Properties
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
        public RelayCommand PlayAlbumCommand
        {
            get
            {
                return this.m_playAlbumCommand ??
                    (this.m_playAlbumCommand = new RelayCommand(PlayAlbum, CanPlayAlbum));
            }
        }
        #endregion

        #region MethodsPublic
        public AlbumDetailPageViewModel(IDataService dataService, IAccountService accountService, INavigationService navigationService, IResourceService resourceService, PlayerManager playerManager, IDialogService dialogService, ICacheableBitmapService cacheableBitmapService)
            : base(dataService, accountService, dialogService, resourceService, cacheableBitmapService, navigationService)
        {
            this.m_playerManager = playerManager;
            Messenger.Default.Register<PlaylistChangeMessage>(this, message =>
            {
                this.LoadPlaylists();
            });
        }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode);
            if (navigationParameter is int)
            {
                this.Album = await this.DataService.GetAlbumById((int)navigationParameter);
                this.CoverSource = this.DataService.GetImage(this.Album.AlbumId);
                this.PlayAlbumCommand.RaiseCanExecuteChanged();
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
        public override void OnNavigatedFrom(bool suspending)
        {
            base.OnNavigatedFrom(suspending);
            this.Album = null;
        }
        #endregion

        #region MethodsProtected
        protected override void PlaySelectedItems()
        {
            base.PlaySelectedItems();
            var selectedItems = this.SelectedItems;
            if (selectedItems != null)
            {
                var selectedTracks = new System.Collections.ObjectModel.ObservableCollection<Track>(selectedItems.Cast<Track>());
                if (selectedTracks != null && selectedTracks.Count() > 0)
                {
                    this.PlayTracks(selectedTracks);
                }
                this.SelectedItems.Clear();
            }
        }
        protected override void AddTracksToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                ObservableCollection<Track> tracks = null;
                var selectedItems = this.SelectedItems;
                if (selectedItems.Count == 0)
                {
                    tracks = new ObservableCollection<Track>(this.Album.Tracks);
                }
                else
                {
                    tracks = new ObservableCollection<Track>(selectedItems.Cast<Track>());
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
            this.SelectedItems.Clear();
        }
        #endregion

        #region MethodsPrivate
        private bool CanPlayAlbum()
        {
            return this.Album != null && this.Album.Tracks != null && this.Album.Tracks.Count() > 0;
        }
        private void PlayAlbum()
        {
            var tracks = new System.Collections.ObjectModel.ObservableCollection<Track>(this.Album.Tracks);
            if (tracks != null && tracks.Count() > 0)
            {
                this.PlayTracks(tracks);
            }
        }
        private void PlayTracks(System.Collections.ObjectModel.ObservableCollection<Track> tracks)
        {
            if (tracks != null)
            {
                var trackIds = tracks.Select(track => track.Id);
                if (trackIds != null)
                {
                    this.m_playerManager.PlayTracks(
                        new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
                        PlayerMode.CD);
                }
            }
        }

        #endregion
    }
}