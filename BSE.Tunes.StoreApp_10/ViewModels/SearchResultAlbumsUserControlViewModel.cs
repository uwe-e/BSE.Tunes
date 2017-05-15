using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultAlbumsUserControlViewModel : FeaturedItemsBaseViewModel
    {
        private Query m_query;

        public Query Query
        {
            get
            {
                return m_query;
            }
            set
            {
                m_query = value;
                RaisePropertyChanged(() => Query);
            }
        }
        public SearchResultAlbumsUserControlViewModel()
        {
        }
        public SearchResultAlbumsUserControlViewModel(Query query): base()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Query = query;
                LoadData();
            }
        }
        public async override void LoadData()
        {
            if (Query != null)
            {
                var albums = await DataService.GetAlbumSearchResults(Query);
                if (albums != null)
                {
                    foreach (var album in albums)
                    {
                        if (album != null)
                        {
                            Items.Add(new GridPanelItemViewModel
                            {
                                Title = album.Title,
                                Subtitle = album.Artist.Name,
                                Data = album,
                                ImageSource = DataService.GetImage(album.AlbumId, true)
                            });
                        }
                    }
                }
            }
        }
        public override void SelectItem(GridPanelItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), item.Data);
        }
        public override async void PlayAll(GridPanelItemViewModel item)
        {
            Album album = item.Data as Album;
            if (album != null)
            {
                album = await DataService.GetAlbumById(album.Id);
                if (album.Tracks != null)
                {
                    var trackIds = album.Tracks.Select(track => track.Id);
                    if (trackIds != null)
                    {
                        PlayerManager.PlayTracks(
                            new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
                            PlayerMode.CD);
                    }
                }
            }
        }
    }
}
