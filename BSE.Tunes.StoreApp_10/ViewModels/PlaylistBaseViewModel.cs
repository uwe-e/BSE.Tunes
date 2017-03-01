using BSE.Tunes.StoreApp.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using System.Collections.Specialized;
using Template10.Services.NavigationService;
using GalaSoft.MvvmLight.Command;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using BSE.Tunes.StoreApp.Mvvm.Messaging;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistBaseViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private bool m_hasSelectedItems;
        private bool m_isCommandBarVisible;
        private ObservableCollection<object> m_selectedItems;
        private RelayCommand m_playSelectedItemsCommand;
        private ICommand m_openPlaylistFlyoutCommand;
        private ICommand m_openAllToPlaylistCommand;
        private bool m_isPlaylistFlyoutOpen;
        private bool m_isAllToPlaylistFlyoutOpen;
        private ObservableCollection<MenuFlyoutItemViewModel> m_playlistMenuItems;
        
        #endregion

        #region Properties
        public bool IsPlaylistFlyoutOpen
        {
            get
            {
                return m_isPlaylistFlyoutOpen;
            }
            set
            {
                m_isPlaylistFlyoutOpen = value;
                RaisePropertyChanged("IsPlaylistFlyoutOpen");
            }
        }
        public bool IsAllToPlaylistFlyoutOpen
        {
            get
            {
                return m_isAllToPlaylistFlyoutOpen;
            }
            set
            {
                m_isAllToPlaylistFlyoutOpen = value;
                RaisePropertyChanged("IsAllToPlaylistFlyoutOpen");
            }
        }
        /// <summary>
        /// Gets or sets an value indication whether the tracklist has at the minimum one selected item.
        /// </summary>
        public virtual bool HasSelectedItems
        {
            get
            {
                return this.m_hasSelectedItems;
            }
            set
            {
                this.m_hasSelectedItems = value;
                RaisePropertyChanged("HasSelectedItems");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the the toolbar for the playlist related content is open.
        /// </summary>
        /// <remarks>Because it´s a twoway bound property in xaml the <see cref="HasSelectedItems"/> property can not used.</remarks>
        public virtual bool IsCommandBarVisible
        {
            get
            {
                return this.m_isCommandBarVisible;
            }
            set
            {
                this.m_isCommandBarVisible = value;
                RaisePropertyChanged("IsCommandBarVisible");
            }
        }
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return this.m_selectedItems;
            }
            set
            {
                this.m_selectedItems = value;
                RaisePropertyChanged("SelectedItems");
            }
        }
        public virtual ObservableCollection<MenuFlyoutItemViewModel> MenuItemsPlaylist
        {
            get
            {
                if (m_playlistMenuItems == null)
                {
                    m_playlistMenuItems = new ObservableCollection<MenuFlyoutItemViewModel>();
                }
                return m_playlistMenuItems;
            }
        }
        public RelayCommand PlaySelectedItemsCommand => m_playSelectedItemsCommand ?? (m_playSelectedItemsCommand = new RelayCommand(PlaySelectedItems, CanPlaySelectedItems));
        public ICommand OpenPlaylistFlyoutCommand => m_openPlaylistFlyoutCommand ?? (m_openPlaylistFlyoutCommand = new RelayCommand(OpenPlaylistFlyout));
        public ICommand OpenAllToPlaylistCommand => m_openAllToPlaylistCommand ?? (m_openAllToPlaylistCommand = new RelayCommand(OpenAllToPlaylistFlyout));

        

        #endregion

        #region MethodsPublic
        public PlaylistBaseViewModel()
        {
            Messenger.Default.Register<PlaylistChangedArgs>(this, args =>
            {
                PlaylistCreatedArgs playlistCreated = args as PlaylistCreatedArgs;
                if (playlistCreated != null)
                {
                    switch (playlistCreated.InsertMode)
                    {
                        case InsertMode.Selected:
                            AddSelectedToPlaylist(args.Playlist);
                            break;
                        case InsertMode.All:
                            AddAllToPlaylist(args.Playlist);
                            break;
                    }
                }
            });
        }
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (this.SelectedItems == null)
            {
                this.SelectedItems = new ObservableCollection<object>();
                this.SelectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
            }
            return Task.CompletedTask;
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (this.SelectedItems != null)
            {
                this.SelectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
                this.SelectedItems = null;
            }
            if (MenuItemsPlaylist != null)
            {
                foreach (var menuItem in MenuItemsPlaylist)
                {
                    menuItem.ItemClicked -= OnMenuItemViewModelClicked;
                }
                MenuItemsPlaylist.Clear();
            }
            this.HasSelectedItems = false;
            this.IsCommandBarVisible = false;
            return base.OnNavigatedFromAsync(state, suspending);
        }
        #endregion

        #region MethodsProtected
        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectedItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.HasSelectedItems = this.SelectedItems.Count > 0;
            this.IsCommandBarVisible = this.HasSelectedItems;
            this.PlaySelectedItemsCommand.RaiseCanExecuteChanged();
            //this.ClearSelectionCommand.RaiseCanExecuteChanged();
        }
        protected virtual async void CreatePlaylistMenu()
        {
            this.MenuItemsPlaylist.Clear();

            User user = SettingsService.Instance.User;
            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                var playlists = await this.DataService.GetPlaylistsByUserName(user.UserName);
                if (playlists != null)
                {
                    this.CreateMenuItems(playlists);
                }
            }
            this.MenuItemsPlaylist.Insert(0, new MenuFlyoutItemViewModel
            {
                IsSeparator = true
            });

            var menuItem = new NewPlaylistFlyoutItemViewModel
            {
                Text = this.ResourceService.GetString("FlyoutMenuItem_AddNewPlaylist", "New Playlist")
            };
            menuItem.ItemClicked += OnMenuItemViewModelClicked;

            this.MenuItemsPlaylist.Insert(0, menuItem);
        }
        protected virtual void CreateMenuItems(ObservableCollection<Playlist> playlists)
        {
            if (playlists != null)
            {
                foreach (var playlist in playlists)
                {
                    if (playlist != null)
                    {
                        var menuItem = new PlaylistFlyoutItemViewModel
                        {
                            Text = playlist.Name,
                            Playlist = playlist
                        };
                        menuItem.ItemClicked += OnMenuItemViewModelClicked;
                        this.MenuItemsPlaylist.Add(menuItem);
                    }
                }
            }
        }
        protected virtual void PlaySelectedItems()
        {
        }
        protected virtual void AddSelectedToPlaylist(Playlist playlist)
        {
        }
        protected virtual void AddAllToPlaylist(Playlist playlist)
        {
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
                        ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                        await cacheableBitmapService.RemoveCache(changedPlaylist.Guid.ToString());
                        //Refreshing all the playlist entry views
                        //Messenger.Default.Send<PlaylistEntryChangeMessage>(new PlaylistEntryChangeMessage { Playlist = changedPlaylist });
                    }
                }
                catch (Exception exception)
                {
                    //DialogService.ShowExDialog(exception.Message);
                }
            }
        }
        #endregion

        #region MethodsPrivate
        private bool CanPlaySelectedItems()
        {
            return this.SelectedItems?.Count > 0;
        }

        private void OpenPlaylistFlyout()
        {
            IsPlaylistFlyoutOpen = true;
        }

        private void OpenAllToPlaylistFlyout()
        {
            IsAllToPlaylistFlyoutOpen = true;
        }
        private void OnMenuItemViewModelClicked(object sender, EventArgs e)
        {
            IsPlaylistFlyoutOpen = false;
            IsAllToPlaylistFlyoutOpen = false;
            SelectedToPlaylist(sender as MenuFlyoutItemViewModel);
        }

        private async void SelectedToPlaylist(MenuFlyoutItemViewModel menuItemViewModel)
        {
            //Necessary because NewPlaylistFlyoutItemViewModel is a own viewmodel.
            NewPlaylistFlyoutItemViewModel viewModel = menuItemViewModel as NewPlaylistFlyoutItemViewModel;
            if (viewModel != null)
            {
                IDialogService dialogService = DialogService.Instance;
                await dialogService.ShowContentDialogAsync(new NewPlaylistContentDialogViewModel
                {
                    InsertMode = menuItemViewModel.InsertMode
                });
            }
            this.ChoosePlaylist(menuItemViewModel);
        }

        private void ChoosePlaylist(MenuFlyoutItemViewModel menuItemViewModel)
        {
            PlaylistFlyoutItemViewModel viewModel = menuItemViewModel as PlaylistFlyoutItemViewModel;
            if (viewModel != null && viewModel.Playlist != null)
            {
                AddToPlaylist(viewModel.Playlist, viewModel.InsertMode);
            }
        }
        private void AddToPlaylist(Playlist playlist, InsertMode insertMode)
        {
            if (playlist != null)
            {
                switch (insertMode)
                {
                    case InsertMode.All:
                        this.AddAllToPlaylist(playlist);
                        break;
                    default:
                        this.AddSelectedToPlaylist(playlist);
                        break;
                }
            }
        }
        #endregion
    }
}
