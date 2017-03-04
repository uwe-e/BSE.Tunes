using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistsPageViewModel : FeaturedItemsBaseViewModel
    {
        #region MethodsPublic
        public async override void LoadData()
        {
            User user = SettingsService.Instance.User;
            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                try
                {
                    ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                    var playlists = await DataService.GetPlaylistsByUserName(user.UserName);
                    foreach (var playlst in playlists)
                    {
                        if (playlst != null)
                        {

                            var playlist = await DataService.GetPlaylistByIdWithNumberOfEntries(playlst.Id, user.UserName);
                            if (playlist != null)
                            {
                                System.Collections.ObjectModel.ObservableCollection<Guid> albumIds = await DataService.GetPlaylistImageIdsById(playlist.Id, user.UserName, 4);
                                Items.Add(new GridPanelItemViewModel
                                {
                                    Title = playlist.Name,
                                    Subtitle = FormatNumberOfEntriesString(playlist),
                                    BitmapSource = await cacheableBitmapService.GetBitmapSource(
                                        new ObservableCollection<Uri>(albumIds.Select(id => DataService.GetImage(id, true))),
                                        playlist.Guid.ToString(),
                                        160, true),
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
        public override void SelectItem(GridPanelItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.PlaylistDetailPage), item.Data);
        }
        #endregion
    }
}
