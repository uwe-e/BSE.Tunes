using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistsPageViewModel : SelectableItemsBaseViewModel
    {
        #region FieldsPrivate
        private bool m_allItemsSelected;
        private bool m_allItemsSelectable;
        private ICommand m_selectAllItemsCommand;
        private RelayCommand m_deleteSelectedItemsCommand;
        private ICommand m_deletePlaylistCommand;
        #endregion

        #region Properties
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
        public User User
        {
            get;
        } = SettingsService.Instance.User;
        public ICommand SelectAllItemsCommand => m_selectAllItemsCommand ?? (m_selectAllItemsCommand = new RelayCommand(SelectAll));
        public ICommand DeletePlaylistCommand => m_deletePlaylistCommand ?? (m_deletePlaylistCommand = new RelayCommand<ListViewItemViewModel>(DeletePlaylist));
        public RelayCommand DeleteSelectedItemsCommand => m_deleteSelectedItemsCommand ?? (m_deleteSelectedItemsCommand = new RelayCommand(DeleteSelectedItems, CanDeleteSelectedItems));
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
            if (!string.IsNullOrEmpty(User.UserName))
            {
                try
                {
                    Items.Clear();
                    ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                    var playlists = await DataService.GetPlaylistsByUserName(User.UserName);
                    if (playlists != null)
                    {
                        foreach (var playlst in playlists)
                        {
                            if (playlst != null)
                            {
                                var playlist = await DataService.GetPlaylistByIdWithNumberOfEntries(playlst.Id, User.UserName);
                                if (playlist != null)
                                {
                                    ObservableCollection<Guid> albumIds = await DataService.GetPlaylistImageIdsById(playlist.Id, User.UserName, 4);
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
                }
                finally
                {
                }
            }
        }
        public override void SelectItem(GridPanelItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.PlaylistDetailPage), item.Data);
        }
        public override void OpenFlyout(GridPanelItemViewModel item)
        {
            //if there are selections, clear it before open the flyout.
            if (!SelectedItems.Contains(item))
            {
                ClearSelection();
            }
            base.OpenFlyout(item);
        }
        public override async void PlayAll(GridPanelItemViewModel item)
        {
            if (HasSelectedItems)
            {
                PlaySelectedItems();
            }
            else
            {
                Playlist playlist = item.Data as Playlist;
                if (playlist != null && !string.IsNullOrEmpty(User.UserName))
                {
                    playlist = await this.DataService.GetPlaylistById(playlist.Id, User.UserName);
                    var entryIds = playlist.Entries.Select(entry => entry.TrackId);
                    if (entryIds != null)
                    {
                        PlayerManager.PlayTracks(
                            new System.Collections.ObjectModel.ObservableCollection<int>(entryIds),
                            PlayerMode.Playlist);
                    }
                }
            }
        }
        public override async void PlaySelectedItems()
        {
            if (!string.IsNullOrEmpty(User.UserName))
            {
                var playlistIds = SelectedItems.Cast<GridPanelItemViewModel>().Select(itm => (Playlist)itm.Data).Select(itm => itm.Id).ToList();
                if (playlistIds != null)
                {
                    var entryIds = await DataService.GetTrackIdsByPlaylistIds(playlistIds, User.UserName);
                    if (entryIds != null)
                    {
                        PlayerManager.PlayTracks(
                            new System.Collections.ObjectModel.ObservableCollection<int>(entryIds),
                            PlayerMode.Playlist);
                    }
                }
            }
        }
        #endregion

        #region MethodsProtected
        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnSelectedItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnSelectedItemsCollectionChanged(sender, e);
            AllItemsSelected = Items.OrderBy(itm => ((Playlist)itm.Data).Id).SequenceEqual(
                SelectedItems.Cast<GridPanelItemViewModel>().OrderBy(itm => ((Playlist)itm.Data).Id));
            AllItemsSelectable = HasSelectedItems & !AllItemsSelected;
        }
        #endregion

        #region MethodsPrivate
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
                DeletePlaylistContentDialogViewModel deletePlaylistDialog = new DeletePlaylistContentDialogViewModel();
                foreach (var playlist in playlistsToDelete)
                {
                    if (playlist != null)
                    {
                        deletePlaylistDialog.Playlists.Add(playlist);
                    }
                }
                deletePlaylistDialog.DeleteInformation = string.Format(CultureInfo.InvariantCulture, ResourceService.GetString("DeletePlaylistContentDialog_TxtDeleteSelectedInformation"));
                IDialogService dialogService = DialogService.Instance;
                await dialogService.ShowContentDialogAsync(deletePlaylistDialog);
            }
        }
        private async void DeletePlaylist(ListViewItemViewModel item)
        {
            if (HasSelectedItems)
            {
                DeleteSelectedItems();
            }
            else
            {
                DeletePlaylistContentDialogViewModel deletePlaylistDialog = new DeletePlaylistContentDialogViewModel();
                deletePlaylistDialog.Playlists.Add(item.Data as Playlist);
                deletePlaylistDialog.DeleteInformation = string.Format(CultureInfo.InvariantCulture, ResourceService.GetString("DeletePlaylistContentDialog_TxtDeleteInformation"), ((Playlist)item.Data).Name);
                IDialogService dialogService = DialogService.Instance;
                await dialogService.ShowContentDialogAsync(deletePlaylistDialog);
            }
        }
        #endregion
    }
}
