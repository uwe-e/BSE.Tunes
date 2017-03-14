using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistsPageViewModel : FeaturedItemsBaseViewModel
    {
        #region FieldsPrivate
        private ObservableCollection<object> m_selectedItems;
        private bool m_isCommandBarVisible;
        private bool m_hasSelectedItems;
        private bool m_allItemsSelected;
        private ICommand m_selectItemsCommand;
        private ICommand m_clearSelectionCommand;
        private ICommand m_selectAllItemsCommand;
        private RelayCommand m_deleteSelectedItemsCommand;
        private RelayCommand m_playSelectedItemsCommand;

        #endregion

        #region Properties
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

        public ICommand SelectItemsCommand => m_selectItemsCommand ?? (m_selectItemsCommand = new RelayCommand<ListViewItemViewModel>(SelectItems));
        public ICommand ClearSelectionCommand => m_clearSelectionCommand ?? (m_clearSelectionCommand = new RelayCommand(ClearSelection));
        public ICommand SelectAllItemsCommand => m_selectAllItemsCommand ?? (m_selectAllItemsCommand = new RelayCommand(SelectAll));
        public RelayCommand DeleteSelectedItemsCommand => m_deleteSelectedItemsCommand ?? (m_deleteSelectedItemsCommand = new RelayCommand(DeleteSelectedItems, CanDeleteSelectedItems));
        public RelayCommand PlaySelectedItemsCommand => m_playSelectedItemsCommand ?? (m_playSelectedItemsCommand = new RelayCommand(PlaySelectedItems, CanPlaySelectedItems));

        
        #endregion

        #region MethodsPublic
        public PlaylistsPageViewModel()
        {
            Messenger.Default.Register<PlaylistChangedArgs>(this, args =>
                        {
                            PlaylistDeletedArgs playlistDeleted = args as PlaylistDeletedArgs;
                            if (playlistDeleted != null)
                            {
                                LoadData();
                            }
                        });
        }
            
        public async override void LoadData()
        {
            User user = SettingsService.Instance.User;
            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                try
                {
                    Items.Clear();
                    ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                    var playlists = await DataService.GetPlaylistsByUserName(user.UserName);
                    foreach (var playlst in playlists)
                    {
                        if (playlst != null)
                        {
                            var playlist = await DataService.GetPlaylistByIdWithNumberOfEntries(playlst.Id, user.UserName);
                            if (playlist != null)
                            {
                                ObservableCollection<Guid> albumIds = await DataService.GetPlaylistImageIdsById(playlist.Id, user.UserName, 4);
                                Items.Add(new GridPanelItemViewModel
                                {
                                    Title = playlist.Name,
                                    Subtitle = FormatNumberOfEntriesString(playlist),
                                    BitmapSource = await cacheableBitmapService.GetBitmapSource(
                                        new ObservableCollection<Uri>(albumIds.Select(id => DataService.GetImage(id, true))),
                                        playlist.Guid.ToString(),
                                        150, true),
                                    Data = playlist
                                });
                            }
                        }
                    }
                }
                finally
                {
                }
            }
        }
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (SelectedItems == null)
            {
                SelectedItems = new ObservableCollection<object>();
                SelectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
            }
            return Task.CompletedTask;
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (SelectedItems != null)
            {
                SelectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
                SelectedItems = null;
            }
            HasSelectedItems = AllItemsSelected = false;
            IsCommandBarVisible = false;
            return base.OnNavigatedFromAsync(state, suspending);
        }
        public override void SelectItem(GridPanelItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.PlaylistDetailPage), item.Data);
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
            AllItemsSelected = Items.OrderBy(itm =>((Playlist)itm.Data).Id).SequenceEqual(
                SelectedItems.Cast<GridPanelItemViewModel>().OrderBy(itm => ((Playlist)itm.Data).Id));
            //PlaySelectedItemsCommand.RaiseCanExecuteChanged();
            //this.ClearSelectionCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region MethodsPrivate
        private void SelectItems(ListViewItemViewModel viewModel)
        {
            HasSelectedItems = true;
            SelectedItems.Add(viewModel);
        }
        private void ClearSelection()
        {
            SelectedItems?.Clear();
        }
        private void SelectAll()
        {
            var notSelectedItems = Items.Except(SelectedItems);
            foreach (var item in notSelectedItems)
            {
                SelectedItems.Add(item);
            }
        }
        private bool CanDeleteSelectedItems()
        {
            return HasSelectedItems = SelectedItems?.Count > 0;
        }

        private async void DeleteSelectedItems()
        {
           var playlistsToDelete = new ObservableCollection<Playlist>(SelectedItems.Cast<GridPanelItemViewModel>().Select(item => item.Data).Cast<Playlist>());
            if (playlistsToDelete != null)
            {
                ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                var hasDeleted = await DataService.DeletePlaylists(playlistsToDelete);
                if (hasDeleted)
                {
                    foreach (var playlist in playlistsToDelete)
                    {
                        if (playlist != null)
                        {
                            await cacheableBitmapService.RemoveCache(playlist.Guid.ToString());
                        }
                    }
                    Messenger.Default.Send<PlaylistChangedArgs>(new PlaylistDeletedArgs(null));
                }
            }
        }
        private bool CanPlaySelectedItems()
        {
            return HasSelectedItems;
        }

        private void PlaySelectedItems()
        {
            var playlists = SelectedItems.Cast<GridPanelItemViewModel>().Select(itm => (Playlist)itm.Data);
        }
        #endregion
    }
}
