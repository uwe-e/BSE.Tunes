using BSE.Tunes.Data;
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
    }
}
