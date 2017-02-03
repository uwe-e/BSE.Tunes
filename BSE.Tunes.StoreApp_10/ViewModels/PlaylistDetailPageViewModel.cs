using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistDetailPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private Playlist m_playlist;
        private BitmapSource m_coverSource;
        private string m_subTitle;
        private RelayCommand m_playAllCommand;
        private PlayerManager m_playerManager;
        private ICommand m_playEntryCommand;
        #endregion

        #region Properties
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
        public BitmapSource CoverSource
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
        public string InfoSubTitle
        {
            get
            {
                return this.m_subTitle;
            }
            set
            {
                this.m_subTitle = value;
                RaisePropertyChanged("InfoSubTitle");
            }
        }
        public RelayCommand PlayAllCommand => m_playAllCommand ?? (m_playAllCommand = new RelayCommand(PlayAll, CanPlayAll));
        public ICommand PlayEntryCommand => m_playEntryCommand ?? (m_playEntryCommand = new RelayCommand<PlaylistEntry>(PlayEntry));
        #endregion

        #region MethodsPublic
        public PlaylistDetailPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                m_playerManager = PlayerManager.Instance;
            }
        }
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Playlist playlist = parameter as Playlist;
            if (playlist != null)
            {
                User user = SettingsService.Instance.User;
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    try
                    {
                        Collection<Uri> imageUris = null;
                        ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                        this.Playlist = await this.DataService.GetPlaylistById(playlist.Id, user.UserName);
                        if (this.Playlist != null)
                        {
                            foreach (var entry in this.Playlist.Entries?.OrderBy(pe => pe.SortOrder))
                            {
                                if (entry != null)
                                {
                                    if (imageUris == null)
                                    {
                                        imageUris = new Collection<Uri>();
                                    }
                                    imageUris.Add(this.DataService.GetImage(entry.AlbumId));
                                }
                            }
                            if (imageUris != null)
                            {
                                this.CoverSource = await cacheableBitmapService.GetBitmapSource(
                                    new ObservableCollection<Uri>(imageUris.Take(4)),
                                    this.Playlist.Guid.ToString(),
                                    500);
                            }
                            this.InfoSubTitle = FormatNumberOfEntriesString(playlist);
                        }
                    }
                    finally
                    {
                        this.PlayAllCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }
        #endregion

        #region MethodsPrivate
        private bool CanPlayAll()
        {
            return this.Playlist?.Entries != null && this.Playlist.Entries.Count() > 0;
        }

        private void PlayAll()
        {
            PlayerMode playerMode = PlayerMode.Playlist;
            var entryIds = this.Playlist.Entries.Select(entry => entry.TrackId);
            if (entryIds != null)
            {
                this.m_playerManager.PlayTracks(
                    new System.Collections.ObjectModel.ObservableCollection<int>(entryIds),
                    playerMode);
            }
        }
        private void PlayEntry(PlaylistEntry entry)
        {
            m_playerManager.PlayTrack(entry.TrackId, PlayerMode.Song);
        }
        private string FormatNumberOfEntriesString(Playlist playlist)
        {
            int numberOfEntries = 0;
            if (playlist != null)
            {
                numberOfEntries = playlist.NumberEntries;
            }
            return string.Format(CultureInfo.CurrentUICulture, "{0} {1}", numberOfEntries, ResourceService.GetString("PlaylistItem_PartNumberOfEntries", "Songs"));
        }
        #endregion
    }
}
