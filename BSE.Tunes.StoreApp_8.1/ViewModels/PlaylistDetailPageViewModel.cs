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
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistDetailPageViewModel : BasePlaylistableViewModel, INavigationAware
    {
        #region FieldsPrivate
        private INavigationService m_navigationService;
        private PlayerManager m_playerManager;
        private Playlist m_playlist;
        private ObservableCollection<PlaylistEntry> m_selectedEntries;
        private ObservableCollection<PlaylistEntry> m_entries;
		private ObservableCollection<Uri> m_uris;
        private bool m_hasSelectedTracks;
        private RelayCommand m_playPlaylistCommand;
        private RelayCommand m_removeSelectedEntriesCommand;
        private RelayCommand m_removeSelectionCommand;
        private ICommand m_goBackCommand;
        private bool m_hasChanged;
        private string m_pageTitle;
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
        public ObservableCollection<PlaylistEntry> SelectedEntries
        {
            get
            {
                return this.m_selectedEntries;
            }
            set
            {
                this.m_selectedEntries = value;
                RaisePropertyChanged("SelectedEntries");
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
		public ObservableCollection<Uri> Uris
		{
			get
			{
				return this.m_uris;
			}
			set
			{
				this.m_uris = value;
				RaisePropertyChanged("Uris");
			}
		}
        public RelayCommand PlayPlaylistCommand
        {
            get
            {
                return this.m_playPlaylistCommand ??
                    (this.m_playPlaylistCommand = new RelayCommand(PlayPlaylist, CanExecutePlayPlaylist));
            }
        }
        public RelayCommand RemoveSelectedEntriesCommand
        {
            get
            {
                return this.m_removeSelectedEntriesCommand ??
                    (this.m_removeSelectedEntriesCommand = new RelayCommand(RemoveSelectedEntries, CanExecuteRemoveSelection));
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

        public PlaylistDetailPageViewModel(IDataService dataService, IAccountService accountService, INavigationService navigationService, PlayerManager playerManager, IResourceService resourceService, IDialogService dialogService)
            : base(dataService, accountService, dialogService, resourceService)
        {
            this.m_navigationService = navigationService;
            this.m_playerManager = playerManager;
        }

        public void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode)
        {
            if (navigationParameter is int)
            {
                TunesUser user = this.AccountService.User;
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    var playlist = Task.Run(() =>
                    {
                        try
                        {
                            return this.DataService.GetPlaylistById((int)navigationParameter, user.UserName);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    });
                    try
                    {
                        playlist.Wait();
                        this.Playlist = playlist.Result;

                        this.PageTitle = string.Format(CultureInfo.CurrentCulture,
                            this.ResourceService.GetString("IDS_PlaylistPage_PageTitle", "Playlist {0}"), this.Playlist.Name);

                        this.PlayPlaylistCommand.RaiseCanExecuteChanged();
                        this.SelectedEntries = new ObservableCollection<PlaylistEntry>();
                        this.SelectedEntries.CollectionChanged += OnSelectedEntryCollectionChanged;
                        if (this.Playlist != null && this.Playlist.Entries != null)
                        {
							foreach (var entry in this.Playlist.Entries.OrderBy(pe => pe.SortOrder))
                            {
                                if (entry != null)
                                {
                                    if (this.Entries == null)
                                    {
                                        this.Entries = new ObservableCollection<PlaylistEntry>();
                                        //this.Entries.CollectionChanged += OnEntryCollectionChanged;
										this.Uris = new ObservableCollection<Uri>();
                                    }
									this.Entries.Add(entry);
									//this.Uris.Add(this.DataService.GetImage(entry.AlbumId, true));
									this.Uris.Add(this.DataService.GetImage(entry.AlbumId));
                                    this.PlayPlaylistCommand.RaiseCanExecuteChanged();
                                }
                            }
                        }
						if (this.Entries != null)
						{
							this.Entries.CollectionChanged += OnEntryCollectionChanged;
						}
						this.MenuItemsPlaylist = new ObservableCollection<MenuItemViewModel>();
                        this.LoadPlaylists();
                    }
                    catch (Exception exception)
                    {
                        this.DialogService.ShowDialog(exception.Message);
                    }
                }
            }
        }

        public void OnNavigatedFrom(bool suspend)
        {
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
                        if (!suspend)
                        {
                            Messenger.Default.Send<PlaylistEntryChangeMessage>(new PlaylistEntryChangeMessage { Playlist = this.Playlist });
                        }
                    }
                    catch{}
                }
                this.Entries.CollectionChanged -= OnEntryCollectionChanged;
                this.Entries = null;
            }
            if (this.SelectedEntries != null)
            {
                this.SelectedEntries.CollectionChanged -= OnSelectedEntryCollectionChanged;
                this.SelectedEntries = null;
            }
            this.Playlist = null;
        }
        #endregion

        #region MethodsProtected
        protected override void AddTracksToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                var playlistEntries = this.SelectedEntries;
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
        protected override void CreateMenuItems(ObservableCollection<Playlist> playlists)
        {
            this.MenuItemsPlaylist.Clear();
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
        #endregion

        #region MethodsPrivate
        private bool CanExecuteRemoveSelection()
        {
            return this.HasSelectedTracks = this.SelectedEntries != null && this.SelectedEntries.Count > 0;
        }
        private void RemoveSelectedEntries()
        {
            var list = this.SelectedEntries.ToList();
            foreach (var entry in list)
            {
                if (entry != null)
                {
                    this.Entries.Remove(entry);
                }
            }
        }
        private void RemoveSelection()
        {
            if (this.SelectedEntries != null)
            {
                this.SelectedEntries.Clear();
            }
        }
        private bool CanExecutePlaySelectedEntries()
        {
            return this.HasSelectedTracks = this.SelectedEntries != null && this.SelectedEntries.Count > 0;
        }
        private bool CanExecutePlayPlaylist()
        {
            return this.Entries != null && this.Entries.Count() > 0;
        }
        private void PlayPlaylist()
        {
			PlayerMode playerMode = PlayerMode.Playlist;
			var entryIds = this.Entries.Select(entry => entry.TrackId);
			var selectedEntryIds = this.SelectedEntries.Select(entry => entry.TrackId);
			if (selectedEntryIds != null && selectedEntryIds.Count() > 0)
			{
				entryIds = selectedEntryIds;
				playerMode = PlayerMode.Song;
			}
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
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach(var item in e.OldItems)
				{
					var entry = item as PlaylistEntry;
					if (entry != null)
					{
						this.Uris.Remove(this.DataService.GetImage(entry.AlbumId, true));
					}
				}
			}
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				foreach (var item in e.NewItems)
				{
					var entry = item as PlaylistEntry;
					if (entry != null)
					{
						this.Uris.Add(this.DataService.GetImage(entry.AlbumId, true));
					}
				}
			}
        }
        private void OnSelectedEntryCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.RemoveSelectedEntriesCommand.RaiseCanExecuteChanged();
            this.RemoveSelectionCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}