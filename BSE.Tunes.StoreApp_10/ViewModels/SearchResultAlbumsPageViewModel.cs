using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Collections;
using BSE.Tunes.StoreApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultAlbumsPageViewModel : SelectableItemsBaseViewModel
    {
        #region FieldsPrivate
        private IncrementalObservableCollection<ListViewItemViewModel> m_albums;
        private string m_headerText;
        private string m_pageHeaderText;
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
        public IncrementalObservableCollection<ListViewItemViewModel> Albums
        {
            get
            {
                return this.m_albums;
            }
            private set
            {
                this.m_albums = value;
                RaisePropertyChanged(() => Albums);
            }
        }
        #endregion

        #region MethodsPublic
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            var query = parameter as Query;
            if (query != null && !string.IsNullOrEmpty(query.SearchPhrase))
            {
                HeaderText = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", query.SearchPhrase);
                PageHeaderText = string.Format(CultureInfo.CurrentUICulture, ResourceService.GetString("SearchResultAlbumsPage_PageHeaderText"), query.SearchPhrase);
                LoadQueryResult(query);
            }
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
        private void LoadQueryResult(Query query)
        {
            int maximumItems = 100;
            int pageIndex = 0;

            Albums = new IncrementalObservableCollection<ListViewItemViewModel>(
                    (uint)maximumItems,
                    (uint count) =>
                    {
                        Func<Task<Windows.UI.Xaml.Data.LoadMoreItemsResult>> taskFunc = async () =>
                        {
                            int pageSize = (int)count;

                            query.PageIndex = pageIndex;
                            query.PageSize = pageSize;

                            var albums = await DataService.GetAlbumSearchResults(query);
                            if (albums != null)
                            {
                                foreach (var album in albums)
                                {
                                    Albums.Add(new GridPanelItemViewModel
                                    {
                                        Title = album.Title,
                                        Subtitle = album.Artist.Name,
                                        ImageSource = DataService?.GetImage(album.AlbumId, true),
                                        Data = album
                                    });
                                }
                                pageIndex += pageSize;
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
        #endregion
    }
}
