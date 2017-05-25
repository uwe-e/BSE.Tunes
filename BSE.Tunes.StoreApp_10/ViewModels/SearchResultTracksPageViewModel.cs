using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultTracksPageViewModel : SelectableItemsBaseViewModel
    {
        #region FieldsPrivate
        private IncrementalObservableCollection<ListViewItemViewModel> m_tracks;
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
        public IncrementalObservableCollection<ListViewItemViewModel> Tracks
        {
            get
            {
                return this.m_tracks;
            }
            private set
            {
                this.m_tracks = value;
                RaisePropertyChanged(() => Tracks);
            }
        }
        #endregion

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            var query = parameter as Query;
            if (query != null && !string.IsNullOrEmpty(query.SearchPhrase))
            {
                HeaderText = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", query.SearchPhrase);
                PageHeaderText = string.Format(CultureInfo.CurrentUICulture, ResourceService.GetString("SearchResultTracksPage_PageHeaderText"), query.SearchPhrase);
                LoadTrackResult(query);
            }
        }
        private void LoadTrackResult(Query query)
        {
            int maximumItems = 100;
            int pageIndex = 0;

            this.Tracks = new IncrementalObservableCollection<ListViewItemViewModel>(
                    (uint)maximumItems,
                    (uint count) =>
                    {
                        Func<Task<Windows.UI.Xaml.Data.LoadMoreItemsResult>> taskFunc = async () =>
                        {
                            int pageSize = (int)count;

                            query.PageIndex = pageIndex;
                            query.PageSize = pageSize;

                            var tracks = await DataService.GetTrackSearchResults(query);
                            if (tracks != null)
                            {
                                foreach (var track in tracks)
                                {
                                    Tracks.Add(new GridPanelItemViewModel
                                    {
                                        Data = track
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
    }
}
