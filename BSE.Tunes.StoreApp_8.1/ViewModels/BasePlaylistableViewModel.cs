using BSE.Tunes.Data;
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

namespace BSE.Tunes.StoreApp.ViewModels
{
    public abstract class BasePlaylistableViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private NewPlaylistUserControlViewModel m_newPlaylistViewModel;
        private ICommand m_selectToPlaylistCommand;
        #endregion

        #region Properties
        public IDataService DataService
        {
            get;
            private set;
        }
        public IAccountService AccountService
        {
            get;
            private set;
        }
        public IResourceService ResourceService
        {
            get;
            private set;
        }
        public IDialogService DialogService
        {
            get;
            private set;
        }
        public ICacheableBitmapService CacheableBitmapService
        {
            get;
            private set;
        }
        //public abstract ObservableCollection<MenuItemViewModel> MenuItemsPlaylist
        //{
        //	get;
        //	set;
        //}
        public virtual ObservableCollection<MenuItemViewModel> MenuItemsPlaylist
        {
            get;
            set;
        }
        public ICommand SelectToPlaylistCommand
        {
            get
            {
                return this.m_selectToPlaylistCommand ??
                    (this.m_selectToPlaylistCommand = new RelayCommand<MenuItemViewModel>(this.SelectToPlaylist));
            }
        }
        public NewPlaylistUserControlViewModel NewPlaylistViewModel
        {
            get { return this.m_newPlaylistViewModel; }
            set
            {
                this.m_newPlaylistViewModel = value;
                RaisePropertyChanged("NewPlaylistViewModel");
            }
        }
        #endregion

        #region MethodsPublic
        public BasePlaylistableViewModel(IDataService dataService, IAccountService accountService, IDialogService dialogService, IResourceService resourceService, ICacheableBitmapService cacheableBitmapService)
        {
            this.DataService = dataService;
            this.AccountService = accountService;
            this.DialogService = dialogService;
            this.ResourceService = resourceService;
            this.CacheableBitmapService = cacheableBitmapService;
        }
        #endregion

        #region Methodsprotected
        protected virtual void LoadPlaylists()
        {
            if (this.AccountService.User != null && !string.IsNullOrEmpty(this.AccountService.User.UserName))
            {
                var taskPlaylist = Task.Run(async () => await this.DataService.GetPlaylistsByUserName(this.AccountService.User.UserName));
                try
                {
                    taskPlaylist.Wait();
                    CreateMenuItems(taskPlaylist.Result);
                }
                catch (Exception exception)
                {
                    this.DialogService.ShowDialog(exception.Message);
                }
            }
        }
        protected virtual void CreateMenuItems(ObservableCollection<Playlist> playlists)
        {
            this.MenuItemsPlaylist.Clear();
            if (playlists != null)
            {
                foreach (var playlist in playlists)
                {
                    if (playlist != null)
                    {
                        this.MenuItemsPlaylist.Add(new MenuPlaylistViewModel { Content = playlist.Name, Playlist = playlist });
                    }
                }
            }
            this.MenuItemsPlaylist.Add(new MenuNewPlaylistViewModel { Content = this.ResourceService.GetString("IDS_NewPlaylist_MenuItem_AddNewPlaylist", "New Playlist") });
        }
        protected virtual void OpenNewPlaylistDialog()
        {
            this.NewPlaylistViewModel = new NewPlaylistUserControlViewModel(this.DataService, this.AccountService, this.ResourceService);
            this.NewPlaylistViewModel.IsOpen = true;
            this.NewPlaylistViewModel.PlaylistInserted += OnNewPlaylistInserted;
        }
        protected virtual void AddTracksToPlaylist(Playlist playlist)
        {
            AppendToPlaylist(playlist);
        }
        protected virtual async void AppendToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                try
                {
                    var changedPlaylist = await this.DataService.AppendToPlaylist(playlist);
                    if (changedPlaylist != null)
                    {
                        await this.CacheableBitmapService.RemoveCache(changedPlaylist.Guid.ToString());
                        //Refreshing all the playlist entry views
                        Messenger.Default.Send<PlaylistEntryChangeMessage>(new PlaylistEntryChangeMessage { Playlist = changedPlaylist });
                    }
                }
                catch (Exception exception)
                {
                    this.DialogService.ShowDialog(exception.Message);
                }
            }
        }
        #endregion

        #region MethodsPrivate
        private void SelectToPlaylist(MenuItemViewModel menuItem)
        {
            MenuNewPlaylistViewModel menuNewPlaylistViewModel = menuItem as MenuNewPlaylistViewModel;
            if (menuNewPlaylistViewModel != null)
            {
                this.OpenNewPlaylistDialog();
            }
            MenuPlaylistViewModel menuPlaylistViewModel = menuItem as MenuPlaylistViewModel;
            if (menuPlaylistViewModel != null && menuPlaylistViewModel.Playlist != null)
            {
                this.AddTracksToPlaylist(menuPlaylistViewModel.Playlist);
            }
        }
        private void OnNewPlaylistInserted(object sender, Event.PlaylistChangedEventArgs e)
        {
            this.NewPlaylistViewModel.PlaylistInserted -= OnNewPlaylistInserted;
            this.NewPlaylistViewModel.IsOpen = false;
            this.NewPlaylistViewModel = null;
            //Refreshing all the playlist views
            Messenger.Default.Send<PlaylistChangeMessage>(new PlaylistChangeMessage());
            Playlist playlist = e.Playlist;
            if (playlist != null)
            {
                this.AddTracksToPlaylist(playlist);
            }
        }
        #endregion
    }
}