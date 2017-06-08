using BSE.Tunes.Data;
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
        #endregion

        #region Properties
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
            if (!string.IsNullOrEmpty(User.UserName))
            {
                try
                {
                    Items.Clear();
                    ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                    var playlists = await DataService.GetPlaylistsByUserName(User.UserName, 6);
                    if (playlists != null)
                    {
                        foreach (var playlst in playlists)
                        {
                            if (playlst != null)
                            {
                                var playlist = await DataService.GetPlaylistByIdWithNumberOfEntries(playlst.Id, User.UserName);
                                if (playlist != null)
                                {
                                    System.Collections.ObjectModel.ObservableCollection<Guid> albumIds = await DataService.GetPlaylistImageIdsById(playlist.Id, User.UserName, 4);
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
                    //this.IsBusy = false;
                }
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
