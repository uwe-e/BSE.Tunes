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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistDetailPageViewModel : BaseTracklistPageViewModel, INavigationAware
    {
        #region FieldsPrivate
        private PlayerManager m_playerManager;
        private Playlist m_playlist;
        private ObservableCollection<PlaylistEntry> m_entries;
        private BitmapSource m_imageSource;
        private RelayCommand m_playPlaylistCommand;
        private RelayCommand m_deleteSelectedEntriesCommand;
        private bool m_hasEntries;
        private bool m_hasChanged;
        private string m_pageTitle;
        #endregion

        #region Properties
        public virtual bool HasEntries
        {
            get
            {
                return this.m_hasEntries;
            }
            set
            {
                this.m_hasEntries = value;
                try
                {
                    RaisePropertyChanged("HasEntries");
                }
                catch { }
            }
        }

        public string PageTitle
        {
            get
            {
                return this.m_pageTitle;
            }
            set
            {
                this.m_pageTitle = value;
                RaisePropertyChanged("PageTitle");
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
        public ObservableCollection<PlaylistEntry> Entries
        {
            get
            {
                return this.m_entries;
            }
            set
            {
                this.m_entries = value;
                RaisePropertyChanged("Entries");
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
        public RelayCommand PlayPlaylistCommand
        {
            get
            {
                return this.m_playPlaylistCommand ??
                    (this.m_playPlaylistCommand = new RelayCommand(PlayPlaylist, CanPlayPlaylist));
            }
        }
        public RelayCommand DeleteSelectedEntriesCommand
        {
            get
            {
                return this.m_deleteSelectedEntriesCommand ??
                    (this.m_deleteSelectedEntriesCommand = new RelayCommand(DeleteSelectedEntries, CanDeleteSelection));
            }
        }
        #endregion

        #region MethodsPublic

        public PlaylistDetailPageViewModel(IDataService dataService, IAccountService accountService, INavigationService navigationService, PlayerManager playerManager, IResourceService resourceService, IDialogService dialogService, ICacheableBitmapService cacheableBitmapService)
            : base(dataService, accountService, dialogService, resourceService, cacheableBitmapService, navigationService)
        {
            this.m_playerManager = playerManager;
            Messenger.Default.Register<PlaylistEntryChangeMessage>(this, message =>
                {
                    Playlist changedPlaylist = message.Playlist;
                    if (changedPlaylist != null && changedPlaylist.Equals(this.Playlist))
                    {
                        //add code for reloading the current playlist when it´s changed by an update
                        //within the playerbar.
                    }
                });
        }

        public async override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode);
            if (navigationParameter is int)
            {
                TunesUser user = this.AccountService.User;
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    try
                    {
                        this.Entries = null;
                        Collection<Uri> imageUris = null;
                        
                        this.Playlist = await this.DataService.GetPlaylistById((int)navigationParameter, user.UserName);
                        if (this.Playlist != null)
                        {
                            this.PageTitle = string.Format(CultureInfo.CurrentCulture,
                                this.ResourceService.GetString("IDS_PlaylistPage_PageTitle", "Playlist {0}"), this.Playlist.Name);

                            if (this.Playlist.Entries != null)
                            {
                                foreach (var entry in this.Playlist.Entries.OrderBy(pe => pe.SortOrder))
                                {
                                    if (entry != null)
                                    {
                                        if (this.Entries == null)
                                        {
                                            this.Entries = new ObservableCollection<PlaylistEntry>();
                                            imageUris = new Collection<Uri>();
                                        }
                                        this.Entries.Add(entry);
                                        imageUris.Add(this.DataService.GetImage(entry.AlbumId));
                                    }
                                }
                                if (imageUris != null)
                                {
                                    this.ImageSource = await this.CacheableBitmapService.GetBitmapSource(
                                        new ObservableCollection<Uri>(imageUris.Take(4)),
                                        this.Playlist.Guid.ToString(),
                                        500);
                                }
                            }
                            if (this.Entries != null)
                            {
                                this.PlayPlaylistCommand.RaiseCanExecuteChanged();
                                this.HasEntries = this.Entries.Count > 0;
                                this.Entries.CollectionChanged += OnEntryCollectionChanged;
                            }
                            this.CreatePlaylistMenu();
                        }
                    }
                    catch (Exception exception)
                    {
                        this.DialogService.ShowDialog(exception.Message);
                    }
                }
            }
        }

        public override void OnNavigatedFrom(bool suspending)
        {
            base.OnNavigatedFrom(suspending);
            if (this.Entries != null)
            {
                if (this.m_hasChanged)
                {
                    this.Playlist.Entries.Clear();
                    foreach (var entry in this.Entries)
                    {
                        if (entry != null)
                        {
                            entry.SortOrder = this.Entries.IndexOf(entry);
                            this.Playlist.Entries.Add(entry);
                        }
                    }
                    try
                    {
                        /*
                         * http://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously
                         * 
                         * It's much simpler to run the task on the thread pool, rather than trying to trick the scheduler to run it synchronously.
                         * That way you can be sure that it won't deadlock. Performance is affected because of the context switch.
                         * 
                         * Task<MyResult> DoSomethingAsync() { ... }
                         * // Starts the asynchronous task on a thread-pool thread.
                         * // Returns a proxy to the original task.
                         * Task<MyResult> task = Task.Run(() => DoSomethingAsync());
                         * 
                         * // Will block until the task is completed...
                         * MyResult result = task.Result; 
                         * 
                        */
                        Task<bool> task = Task.Run(() => this.DataService.UpdatePlaylistEntries(this.Playlist));
                        var ttest = task.Result;
                        Task<bool> task2 = Task.Run(() => this.CacheableBitmapService.RemoveCache(this.Playlist.Guid.ToString()));
                        var tt = task2.Result;
                        if (!suspending)
                        {
                            Messenger.Default.Send<PlaylistEntryChangeMessage>(new PlaylistEntryChangeMessage { Playlist = this.Playlist });
                        }
                    }
                    catch { }
                }
                this.Entries.CollectionChanged -= OnEntryCollectionChanged;
                this.Entries = null;
            }
            this.ImageSource = null;
            this.Playlist = null;
        }
        #endregion

        #region MethodsProtected
        protected override void PlaySelectedItems()
        {
            base.PlaySelectedItems();
            var selectedItems = this.SelectedItems;
            if (selectedItems != null)
            {
                var selectedEntries = new ObservableCollection<PlaylistEntry>(this.SelectedItems.Cast<PlaylistEntry>());
                var selectedEntryIds = selectedEntries.Select(entry => entry.TrackId);
                if (selectedEntryIds != null && selectedEntryIds.Count() > 0)
                {
                    this.m_playerManager.PlayTracks(
                        new System.Collections.ObjectModel.ObservableCollection<int>(selectedEntryIds),
                        PlayerMode.Song);
                }
                this.SelectedItems.Clear();
            }
        }
        protected override void AddTracksToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                var selectedItems = this.SelectedItems;
                if (selectedItems != null)
                {
                    var playlistEntries = new ObservableCollection<PlaylistEntry>(selectedItems.Cast<PlaylistEntry>());
                    if (playlistEntries.Count == 0)
                    {
                        playlistEntries = this.Entries;
                    }
                    if (playlistEntries != null)
                    {
                        foreach (var entry in playlistEntries)
                        {
                            if (entry != null)
                            {
                                playlist.Entries.Add(new PlaylistEntry
                                {
                                    PlaylistId = playlist.Id,
                                    TrackId = entry.TrackId,
                                    Guid = Guid.NewGuid()
                                });
                            }
                        }
                    }
                    base.AddTracksToPlaylist(playlist);
                }
            }
            this.SelectedItems.Clear();
        }
        protected override void CreateMenuItems(ObservableCollection<Playlist> playlists)
        {
            if (playlists != null)
            {
                foreach (var playlist in playlists)
                {
                    if (playlist != null && !playlist.Equals(this.Playlist))
                    {
                        this.MenuItemsPlaylist.Add(new MenuPlaylistViewModel { Content = playlist.Name, Playlist = playlist });
                    }
                }
            }
            this.MenuItemsPlaylist.Add(new MenuNewPlaylistViewModel { Content = this.ResourceService.GetString("IDS_NewPlaylist_MenuItem_AddNewPlaylist", "New Playlist") });
        }
        protected override void OnSelectedItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnSelectedItemsCollectionChanged(sender, e);
            this.DeleteSelectedEntriesCommand.RaiseCanExecuteChanged();
        }
        #endregion


        #region MethodsPrivate
        private bool CanDeleteSelection()
        {
            return this.SelectedItems != null && this.SelectedItems.Count > 0;
        }
        private void DeleteSelectedEntries()
        {
            var selectedItems = this.SelectedItems;
            if (selectedItems != null)
            {
                var list = this.SelectedItems.ToList();
                foreach (var item in list)
                {
                    var entry = item as PlaylistEntry;
                    if (entry != null)
                    {
                        this.Entries.Remove(entry);
                    }
                }
            }
        }
        private bool CanPlayPlaylist()
        {
            return this.Entries != null && this.Entries.Count() > 0;
        }
        private void PlayPlaylist()
        {
            PlayerMode playerMode = PlayerMode.Playlist;
            var entryIds = this.Entries.Select(entry => entry.TrackId);
            if (entryIds != null)
            {
                this.m_playerManager.PlayTracks(
                    new System.Collections.ObjectModel.ObservableCollection<int>(entryIds),
                    playerMode);
            }
        }
        private void OnEntryCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.m_hasChanged = true;
            this.HasEntries = this.Entries.Count > 0;
            this.PlayPlaylistCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}