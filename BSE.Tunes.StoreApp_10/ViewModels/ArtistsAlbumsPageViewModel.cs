using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using BSE.Tunes.StoreApp.Models;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ArtistsAlbumsPageViewModel : SelectableItemsBaseViewModel
    {
        #region FieldsPrivate
        private IncrementalObservableCollection<ListViewItemViewModel> m_albums;
        private string m_headerText;
        private string m_pageHeaderText;
        private Artist m_artist;
        #endregion

        #region Properties
        public string HeaderText
        {
            get
            {
                return m_headerText;
            }
            set
            {
                m_headerText = value;
                RaisePropertyChanged(() => HeaderText);
            }
        }
        public string PageHeaderText
        {
            get
            {
                return m_pageHeaderText;
            }
            set
            {
                m_pageHeaderText = value;
                RaisePropertyChanged(() => PageHeaderText);
            }
        }
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
        #endregion

        #region MethodsPublic

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            LoadData(parameter as Artist);
        }
        public override void SelectItem(GridPanelItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), item.Data);
        }
        public async override void PlayAll(GridPanelItemViewModel item)
        {
            if (HasSelectedItems)
            {
                PlaySelectedItems();
            }
            else
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
        public async override void PlaySelectedItems()
        {
            var albumIds = SelectedItems.Cast<GridPanelItemViewModel>().Select(itm => (Album)itm.Data).Select(itm => itm.Id).ToList();
            if (albumIds != null)
            {
                var entryIds = await DataService.GetTrackIdsByAlbumIds(albumIds);
                if (entryIds != null)
                {
                    PlayerManager.PlayTracks(
                        new System.Collections.ObjectModel.ObservableCollection<int>(entryIds),
                        PlayerMode.CD);
                }
            }
            ClearSelection();
        }
        #endregion

        #region MethodsPrivate
        private async void LoadData(Artist artist)
        {
            Albums = null;
            if (artist != null)
            {
                Artist = artist;

                int numberOfAlbums = await DataService?.GetNumberOfAlbumsByArtist(Artist.Id);
                int pageNumber = 0;

                this.Albums = new IncrementalObservableCollection<ListViewItemViewModel>(
                    (uint)numberOfAlbums,
                    (uint count) =>
                    {
                        Func<Task<Windows.UI.Xaml.Data.LoadMoreItemsResult>> taskFunc = async () =>
                        {
                            int pageSize = (int)count;
                            ObservableCollection<Album> albums = await DataService?.GetAlbumsByArtist(Artist.Id, pageNumber, pageSize);
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
        #endregion
    }
}
