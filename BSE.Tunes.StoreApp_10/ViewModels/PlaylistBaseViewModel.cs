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
using BSE.Tunes.StoreApp.Managers;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistBaseViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private bool m_hasSelectedItems;
        private bool m_allItemsSelectable;
        private bool m_allItemsSelected;
        private bool m_isCommandBarVisible;
        private ObservableCollection<object> m_selectedItems;
        private RelayCommand m_playSelectedItemsCommand;
        private ICommand m_playTrackCommand;
        private RelayCommand m_playAllCommand;
        private ICommand m_openPlaylistFlyoutCommand;
        private ICommand m_openAllToPlaylistCommand;
        private ICommand m_clearSelectionCommand;
        private ICommand m_showFlyoutCommand;
        private ICommand m_selectItemsCommand;
        private ICommand m_selectAllItemsCommand;
        private RelayCommand m_deleteSelectedItemsCommand;
        private bool m_isPlaylistFlyoutOpen;
        private bool m_isAllToPlaylistFlyoutOpen;
        private ObservableCollection<ListViewItemViewModel> m_listViewItems;
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
                return m_hasSelectedItems;
            }
            set
            {
                m_hasSelectedItems = value;
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
                return m_isCommandBarVisible;
            }
            set
            {
                m_isCommandBarVisible = value;
                RaisePropertyChanged("IsCommandBarVisible");
            }
        }
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return m_selectedItems;
            }
            set
            {
                m_selectedItems = value;
                RaisePropertyChanged("SelectedItems");
            }
        }
        public bool AllItemsSelectable
        {
            get
            {
                return m_allItemsSelectable;
            }
            set
            {
                m_allItemsSelectable = value;
                RaisePropertyChanged("AllItemsSelectable");
            }
        }
        public virtual bool AllItemsSelected
        {
            get
            {
                return m_allItemsSelected;
            }
            set
            {
                m_allItemsSelected = value;
                RaisePropertyChanged("AllItemsSelected");
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
        public PlayerManager PlayerManager {get;} = PlayerManager.Instance;
        public ObservableCollection<ListViewItemViewModel> Items => m_listViewItems ?? (m_listViewItems = new ObservableCollection<ListViewItemViewModel>());
        public RelayCommand PlaySelectedItemsCommand => m_playSelectedItemsCommand ?? (m_playSelectedItemsCommand = new RelayCommand(PlaySelectedItems, CanPlaySelectedItems));
        public ICommand PlayTrackCommand => m_playTrackCommand ?? (m_playTrackCommand = new RelayCommand<ListViewItemViewModel>(PlayTrack, CanPlayTrack));
        public RelayCommand PlayAllCommand => m_playAllCommand ?? (m_playAllCommand = new RelayCommand(PlayAll, CanPlayAll));
        public ICommand OpenPlaylistFlyoutCommand => m_openPlaylistFlyoutCommand ?? (m_openPlaylistFlyoutCommand = new RelayCommand(OpenPlaylistFlyout));
        public ICommand OpenAllToPlaylistCommand => m_openAllToPlaylistCommand ?? (m_openAllToPlaylistCommand = new RelayCommand(OpenAllToPlaylistFlyout));
        public ICommand ClearSelectionCommand => m_clearSelectionCommand ?? (m_clearSelectionCommand = new RelayCommand(ClearSelection));
        public ICommand ShowFlyoutCommand => m_showFlyoutCommand ?? (m_showFlyoutCommand = new RelayCommand<ListViewItemViewModel>(ShowFlyout));
        public ICommand SelectItemsCommand => m_selectItemsCommand ?? (m_selectItemsCommand = new RelayCommand<ListViewItemViewModel>(SelectItems));
        public ICommand SelectAllItemsCommand => m_selectAllItemsCommand ?? (m_selectAllItemsCommand = new RelayCommand(SelectAll));
        public RelayCommand DeleteSelectedItemsCommand => m_deleteSelectedItemsCommand ?? (m_deleteSelectedItemsCommand = new RelayCommand(DeleteSelectedItems, CanDeleteSelectedItems));
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
                PlaylistEntriesChangedArgs playlistEntriesChanged = args as PlaylistEntriesChangedArgs;
                if (playlistEntriesChanged != null)
                {
                    CreatePlaylistMenu();
                }
            });
        }
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (SelectedItems == null)
            {
                SelectedItems = new ObservableCollection<object>();
                SelectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
            }
            CreatePlaylistMenu();
            return Task.CompletedTask;
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (SelectedItems != null)
            {
                SelectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
                SelectedItems = null;
            }
            if (MenuItemsPlaylist != null)
            {
                foreach (var menuItem in MenuItemsPlaylist)
                {
                    menuItem.ItemClicked -= OnMenuItemViewModelClicked;
                }
                MenuItemsPlaylist.Clear();
            }
            HasSelectedItems = AllItemsSelected = false;
            IsCommandBarVisible = false;
            return base.OnNavigatedFromAsync(state, suspending);
        }
        public virtual bool CanPlayAll()
        {
            return false;
        }
        public virtual void PlayAll()
        {
        }
        public virtual bool CanPlayTrack(ListViewItemViewModel item)
        {
            return !HasSelectedItems;
        }
        public virtual void PlayTrack(ListViewItemViewModel item)
        {
            PlayerManager.PlayTrack(((Track)item.Data).Id, PlayerMode.Song);
        }
        public virtual void ClearSelection()
        {
            SelectedItems?.Clear();
        }
        public virtual bool CanDeleteSelectedItems()
        {
            return HasSelectedItems;
        }

        public virtual void DeleteSelectedItems()
        {
        }
        public virtual void SelectAll()
        {
            var notSelectedItems = Items.Except(SelectedItems);
            foreach (var item in notSelectedItems)
            {
                SelectedItems.Add(item);
            }
        }
        public virtual void SelectItems(ListViewItemViewModel item)
        {
            HasSelectedItems = true;
            SelectedItems.Add(item);
        }
        public virtual void ShowFlyout(ListViewItemViewModel item)
        {
            //if there are selections, clear it before open the flyout.
            if (!SelectedItems.Contains(item))
            {
                ClearSelection();
            }
            item.IsOpen = true;
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
            HasSelectedItems = SelectedItems.Count > 0;
            IsCommandBarVisible = HasSelectedItems;
            PlaySelectedItemsCommand.RaiseCanExecuteChanged();
            //this.ClearSelectionCommand.RaiseCanExecuteChanged();
        }
        protected virtual async void CreatePlaylistMenu()
        {
            MenuItemsPlaylist.Clear();

            User user = SettingsService.Instance.User;
            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                var playlists = await DataService.GetPlaylistsByUserName(user.UserName);
                if (playlists != null)
                {
                    CreateMenuItems(playlists);
                }
            }
            MenuItemsPlaylist.Insert(0, new MenuFlyoutItemViewModel
            {
                IsSeparator = true
            });

            var menuItem = new NewPlaylistFlyoutItemViewModel
            {
                Text = ResourceService.GetString("FlyoutMenuItem_AddNewPlaylist", "New Playlist")
            };
            menuItem.ItemClicked += OnMenuItemViewModelClicked;

            MenuItemsPlaylist.Insert(0, menuItem);
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
                        MenuItemsPlaylist.Add(menuItem);
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
                    var changedPlaylist = await DataService.AppendToPlaylist(playlist);
                    if (changedPlaylist != null)
                    {
                        ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                        await cacheableBitmapService.RemoveCache(changedPlaylist.Guid.ToString());
                        //Refreshing all the playlist entry views
                        Messenger.Default.Send<PlaylistChangedArgs>(new PlaylistEntriesChangedArgs(changedPlaylist));
                    }
                }
                catch (Exception exception)
                {
                    IDialogService dialogService = DialogService.Instance;
                    await dialogService.ShowMessageDialogAsync(exception.Message);
                }
            }
        }
        #endregion

        #region MethodsPrivate
        private bool CanPlaySelectedItems()
        {
            return SelectedItems?.Count > 0;
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
            ChoosePlaylist(menuItemViewModel);
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
                        AddAllToPlaylist(playlist);
                        break;
                    default:
                        AddSelectedToPlaylist(playlist);
                        break;
                }
            }
        }
        #endregion
    }
}
