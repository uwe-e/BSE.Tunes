using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class AlbumDetailPageViewModel : PlaylistBaseViewModel
    {
        #region FieldsPrivate
        private Album m_album;
        private Uri m_coverSource;
        private PlayerManager m_playerManager;
        private RelayCommand m_playAllCommand;
        private ICommand m_playTrackCommand;
        private ICommand m_selectItemsCommand;
        private ICommand m_showFlyoutCommand;
        private ObservableCollection<ListViewItemViewModel> m_listViewItems;
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
        public ObservableCollection<ListViewItemViewModel> Items => m_listViewItems ?? (m_listViewItems = new ObservableCollection<ListViewItemViewModel>());
        public RelayCommand PlayAllCommand => m_playAllCommand ?? (m_playAllCommand = new RelayCommand(PlayAll, CanPlayAll));
        public ICommand PlayTrackCommand => m_playTrackCommand ?? (m_playTrackCommand = new RelayCommand<ListViewItemViewModel>(PlayTrack, CanPlayTrack));
        public ICommand ShowFlyoutCommand => m_showFlyoutCommand ?? (m_showFlyoutCommand = new RelayCommand<ListViewItemViewModel>(ShowFlyout));
        public ICommand SelectItemsCommand => m_selectItemsCommand ?? (m_selectItemsCommand = new RelayCommand<ListViewItemViewModel>(SelectItems));

        #endregion

        #region MethodsPublic
        public AlbumDetailPageViewModel()
        {
            m_playerManager = PlayerManager.Instance;
        }
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            Album album = parameter as Album;
            if (album != null)
            {
                this.Album = await DataService.GetAlbumById(album.Id);

                foreach (Track track in this.Album.Tracks)
                {
                    if (track != null)
                    {
                        this.Items.Add(new ListViewItemViewModel { Data = track });
                    }
                }
                this.CoverSource = DataService.GetImage(album.AlbumId);
                this.PlayAllCommand.RaiseCanExecuteChanged();

                CreatePlaylistMenu();
            }
        }
        #endregion

        #region MethodsProtected
        protected override void PlaySelectedItems()
        {
            var selectedItems = this.SelectedItems;
            if (selectedItems != null)
            {
                var selectedTracks = new System.Collections.ObjectModel.ObservableCollection<Track>(selectedItems.Cast<ListViewItemViewModel>().Select(itm => itm.Data).Cast<Track>());
                if (selectedTracks?.Count() > 0)
                {
                    this.PlayTracks(selectedTracks);
                }
                this.SelectedItems.Clear();
            }
        }
        protected override void AddAllToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                var tracks = new ObservableCollection<Track>(this.Album.Tracks);
                if (tracks != null)
                {
                    this.AddTracksToPlaylist(playlist, tracks);
                }
            }
        }
        protected override void AddSelectedToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                var selectedItems = this.SelectedItems;
                if (selectedItems != null)
                {
                    var tracks = new ObservableCollection<Track>(selectedItems.Cast<ListViewItemViewModel>()
                        .Select(itm => itm.Data).Cast<Track>());
                    if (tracks != null)
                    {
                        this.AddTracksToPlaylist(playlist, tracks);
                    }
                }
            }
            this.SelectedItems?.Clear();
        }
        #endregion

        #region MethodsPrivate
        private bool CanPlayAll()
        {
            return this.Album != null && this.Album.Tracks != null && this.Album?.Tracks?.Count() > 0;
        }
        private void PlayAll()
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
        private bool CanPlayTrack(ListViewItemViewModel arg)
        {
            return !HasSelectedItems;
        }
        private void PlayTrack(ListViewItemViewModel listviewItem)
        {
            this.m_playerManager.PlayTrack(((Track)listviewItem.Data).Id, PlayerMode.Song);
        }
        private void ShowFlyout(ListViewItemViewModel viewModel)
        {
            viewModel.IsOpen = true;
        }
        private void SelectItems(ListViewItemViewModel viewModel)
        {
            HasSelectedItems = true;
            this.SelectedItems.Add(viewModel);
        }
        private void AddTracksToPlaylist(Playlist playlist, ObservableCollection<Track> tracks)
        {
            if (playlist != null && tracks != null)
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
                this.AppendToPlaylist(playlist);
            }
        }
        #endregion
    }
}
