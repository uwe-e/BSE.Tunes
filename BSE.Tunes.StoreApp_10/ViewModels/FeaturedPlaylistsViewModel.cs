using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Models;
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
    public class FeaturedPlaylistsViewModel : FeaturedItemsBaseViewModel
    {
        #region MethodsPublic
        public override async void LoadData()
        {
            User user = SettingsService.Instance.User;
            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                //this.IsBusy = true;
                //this.Playlists.Clear();
                try
                {
                    this.ItemsGroup = new ItemsGroupViewModel();
                    ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                    var playlists = await DataService.GetPlaylistsByUserName(user.UserName, 6);
                    foreach (var playlist in playlists)
                    {
                        if (playlist != null)
                        {

                            var playlist1 = await DataService.GetPlaylistByIdWithNumberOfEntries(playlist.Id, user.UserName);
                            if (playlist1 != null)
                            {
                                System.Collections.ObjectModel.ObservableCollection<Guid> albumIds = await DataService.GetPlaylistImageIdsById(playlist1.Id, user.UserName, 4);
                                this.ItemsGroup.Items.Add(new ItemViewModel
                                {
                                    Title = playlist1.Name,
                                    Subtitle = FormatNumberOfEntriesString(playlist1),
                                    BitmapSource = await cacheableBitmapService.GetBitmapSource(
                                        new ObservableCollection<Uri>(albumIds.Select(id => DataService.GetImage(id, true))),
                                        playlist1.Guid.ToString(),
                                        160, true)
                                });
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
        #endregion

        private string FormatNumberOfEntriesString(Playlist playlist)
        {
            int numberOfEntries = 0;
            if (playlist != null)
            {
                numberOfEntries = playlist.NumberEntries;
            }
            return string.Format(CultureInfo.CurrentUICulture, "{0} {1}", numberOfEntries, ResourceService.GetString("PlaylistItem_PartNumberOfEntries", "Songs"));
        }
    }
}
