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
        private NewPlaylistUserControlViewModel m_newSelectedToPlaylistViewModel;
        private NewPlaylistUserControlViewModel m_newCompleteToPlaylistViewModel;
        private ICommand m_selectedToPlaylistCommand;
        private ICommand m_completeToPlaylistCommand;
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
        public virtual ObservableCollection<MenuItemViewModel> MenuItemsPlaylist
        {
            get;
            set;
        }
        public ICommand SelectedToPlaylistCommand
        {
            get
            {
                return this.m_selectedToPlaylistCommand ??
                    (this.m_selectedToPlaylistCommand = new RelayCommand<MenuItemViewModel>(this.SelectedToPlaylist));
            }
        }
        public ICommand CompleteToPlaylistCommand
        {
            get
            {
                return this.m_completeToPlaylistCommand ??
                    (this.m_completeToPlaylistCommand = new RelayCommand<MenuItemViewModel>(this.CompleteToPlaylist));
            }
        }
        public NewPlaylistUserControlViewModel NewSelectedToPlaylistViewModel
        {
            get { return this.m_newSelectedToPlaylistViewModel; }
            set
            {
                this.m_newSelectedToPlaylistViewModel = value;
                RaisePropertyChanged("NewSelectedToPlaylistViewModel");
            }
        }
        public NewPlaylistUserControlViewModel NewCompleteToPlaylistViewModel
        {
            get { return this.m_newCompleteToPlaylistViewModel; }
            set
            {
                this.m_newCompleteToPlaylistViewModel = value;
                RaisePropertyChanged("NewCompleteToPlaylistViewModel");
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
        protected virtual async void CreatePlaylistMenu()
        {
            this.MenuItemsPlaylist = new ObservableCollection<MenuItemViewModel>();
            if (this.AccountService.User != null && !string.IsNullOrEmpty(this.AccountService.User.UserName))
            {
                var playlists = await this.DataService.GetPlaylistsByUserName(this.AccountService.User.UserName);
                if (playlists != null)
                {
                    this.CreateMenuItems(playlists);
                }
            }
        }
        protected virtual void CreateMenuItems(ObservableCollection<Playlist> playlists)
        {
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
            this.MenuItemsPlaylist.Add(
                new MenuNewPlaylistViewModel
                {
                    Content = this.ResourceService.GetString("IDS_NewPlaylist_MenuItem_AddNewPlaylist", "New Playlist")
                });
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
        private void CompleteToPlaylist(MenuItemViewModel menuItem)
        {
            MenuNewPlaylistViewModel menuNewPlaylistViewModel = menuItem as MenuNewPlaylistViewModel;
            if (menuNewPlaylistViewModel != null)
            {
                this.NewCompleteToPlaylistViewModel = this.CreateNewPlaylistModel();
            }
            this.AddTracksToNewPlaylist(menuItem);
        }
        private void SelectedToPlaylist(MenuItemViewModel menuItem)
        {
            MenuNewPlaylistViewModel menuNewPlaylistViewModel = menuItem as MenuNewPlaylistViewModel;
            if (menuNewPlaylistViewModel != null)
            {
                this.NewSelectedToPlaylistViewModel = this.CreateNewPlaylistModel();
            }
            this.AddTracksToNewPlaylist(menuItem);
        }
        private void AddTracksToNewPlaylist(MenuItemViewModel menuItem)
        {
            MenuPlaylistViewModel menuPlaylistViewModel = menuItem as MenuPlaylistViewModel;
            if (menuPlaylistViewModel != null && menuPlaylistViewModel.Playlist != null)
            {
                this.AddTracksToPlaylist(menuPlaylistViewModel.Playlist);
            }
        }
        private void OnNewPlaylistInserted(object sender, Event.PlaylistChangedEventArgs e)
        {
            var model = sender as NewPlaylistUserControlViewModel;
            if (model != null)
            {
                model.IsOpen = false;
                model.PlaylistInserted -= OnNewPlaylistInserted;
            }
            
            Playlist playlist = e.Playlist;
            if (playlist != null)
            {
                Messenger.Default.Send<PlaylistChangeMessage>(new PlaylistChangeMessage());
                this.AddTracksToPlaylist(playlist);
            }
        }
        private NewPlaylistUserControlViewModel CreateNewPlaylistModel()
        {
            var newPlaylistViewModel = new NewPlaylistUserControlViewModel(this.DataService, this.AccountService, this.ResourceService)
            {
                IsOpen = true
            };
            newPlaylistViewModel.PlaylistInserted += this.OnNewPlaylistInserted;
            return newPlaylistViewModel;
        }
        #endregion
    }
}