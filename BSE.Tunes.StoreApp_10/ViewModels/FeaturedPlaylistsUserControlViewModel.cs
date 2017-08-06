using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Collections;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
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

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class FeaturedPlaylistsUserControlViewModel : FeaturedItemsBaseViewModel
    {
        #region FieldsPrivate
        private ICommand m_showDeletePlaylistDialogCommand;
        private IncrementalObservableCollection<ListViewItemViewModel> m_playlists;
        #endregion

        #region Properties
        public IncrementalObservableCollection<ListViewItemViewModel> Playlists
        {
            get
            {
                return m_playlists;
            }
            private set
            {
                m_playlists = value;
                RaisePropertyChanged("Playlists");
            }
        }
        public User User
        {
            get;
        } = SettingsService.Instance.User;
        public ICommand ShowDeletePlaylistDialogCommand => m_showDeletePlaylistDialogCommand ?? (m_showDeletePlaylistDialogCommand = new RelayCommand<GridPanelItemViewModel>(ShowDeletePlaylistDialog));
        #endregion

        #region MethodsPublic
        public FeaturedPlaylistsUserControlViewModel()
        {
            Messenger.Default.Register<PlaylistChangedArgs>(this, args =>
            {
                LoadData();
            });
        }
        public override void SelectItem(GridPanelItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.PlaylistDetailPage), item.Data);
        }
        public override void NavigateTo()
        {
            NavigationService.NavigateAsync(typeof(Views.PlaylistsPage));
        }
        public override async void LoadData()
        {
            Playlists = null;
            ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
            if (!string.IsNullOrEmpty(User.UserName))
            {
                int numberOfPlaylists = await DataService?.GetNumberOfPlaylistsByUserName(User.UserName);
                numberOfPlaylists = numberOfPlaylists > 20 ? 20 : numberOfPlaylists;
                int pageNumber = 0;

                Playlists = new IncrementalObservableCollection<ListViewItemViewModel>(
                    (uint)numberOfPlaylists,
                    (uint count) =>
                    {
                        Func<Task<Windows.UI.Xaml.Data.LoadMoreItemsResult>> taskFunc = async () =>
                        {
                            int pageSize = (int)count;

                            ObservableCollection<Playlist> playlists = await DataService?.GetPlaylistsByUserName(User.UserName, pageNumber, pageSize);
                            if (playlists != null)
                            {
                                foreach (var playlst in playlists)
                                {
                                    if (playlst != null)
                                    {
                                        var playlist = await DataService.GetPlaylistByIdWithNumberOfEntries(playlst.Id, User.UserName);
                                        if (playlst != null)
                                        {
                                            System.Collections.ObjectModel.ObservableCollection<Guid> albumIds = await DataService.GetPlaylistImageIdsById(playlist.Id, User.UserName, 4);
                                            Playlists.Add(new GridPanelItemViewModel
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
                                pageNumber += pageSize;
                            }
                            return new Windows.UI.Xaml.Data.LoadMoreItemsResult()
                            {
                                Count = (uint)count
                            };
                        };
                        Task<Windows.UI.Xaml.Data.LoadMoreItemsResult> loadMoreItemsTask = taskFunc();
                        return loadMoreItemsTask.AsAsyncOperation<Windows.UI.Xaml.Data.LoadMoreItemsResult>();
                    }
                );
            }
        }
        public async override void PlayAll(GridPanelItemViewModel item)
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
        #endregion

        #region MethodsPrivate
        private async void ShowDeletePlaylistDialog(GridPanelItemViewModel item)
        {
            DeletePlaylistContentDialogViewModel deletePlaylistDialog = new DeletePlaylistContentDialogViewModel();
            deletePlaylistDialog.Playlists.Add(item.Data as Playlist);
            deletePlaylistDialog.DeleteInformation = string.Format(CultureInfo.InvariantCulture, ResourceService.GetString("DeletePlaylistContentDialog_TxtDeleteInformation"), ((Playlist)item.Data).Name);
            IDialogService dialogService = DialogService.Instance;
            await dialogService.ShowContentDialogAsync(deletePlaylistDialog);
        }
        #endregion
    }
}
