using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ArtistsAlbumsUserControlViewModel : FeaturedItemsBaseViewModel
    {
        private Artist m_artist;
        private IncrementalObservableCollection<ListViewItemViewModel> m_albums;

        public Artist Artist
        {
            get
            {
                return m_artist;
            }
            set
            {
                m_artist = value;
                RaisePropertyChanged(() => Artist);
            }
        }

        public IncrementalObservableCollection<ListViewItemViewModel> Albums
        {
            get
            {
                return this.m_albums;
            }
            private set
            {
                this.m_albums = value;
                RaisePropertyChanged("Albums");
            }
        }

        public ArtistsAlbumsUserControlViewModel()
        {
        }
        public ArtistsAlbumsUserControlViewModel(Artist artist)
        {
            Artist = artist;
            LoadData();
        }
        public async override void LoadData()
        {
            this.Albums = null;
            if (Artist != null)
            {
                int numberOfAlbums = await DataService?.GetNumberOfAlbumsByArtist(Artist.Id);
                numberOfAlbums = numberOfAlbums > 20 ? 20 : numberOfAlbums;
                //int maxItems = 10;
                int pageNumber = 0;

                this.Albums = new IncrementalObservableCollection<ListViewItemViewModel>(
                    (uint)numberOfAlbums,
                    (uint count) =>
                    {
                        Func<Task<Windows.UI.Xaml.Data.LoadMoreItemsResult>> taskFunc = async () =>
                        {
                            int pageSize = (int)count;

                            ObservableCollection<Album> albums = await DataService?.GetAlbumsByArtist(new Query
                            {
                                Data = Artist,
                                PageIndex = pageNumber,
                                PageSize = pageSize
                            });
                            if (albums != null)
                            {
                                foreach (var album in albums)
                                {
                                    this.Albums.Add(new GridPanelItemViewModel
                                    {
                                        Title = album.Title,
                                        Subtitle = album.Artist.Name,
                                        ImageSource = DataService?.GetImage(album.AlbumId, true),
                                        Data = album
                                    });
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
    }
}
