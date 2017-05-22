using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultTracksPageViewModel : SearchResultTracksUserControlViewModel
    {
        #region FieldsPrivate
        private IncrementalObservableCollection<ListViewItemViewModel> m_tracks;
        #endregion

        #region Properties
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
            Query = parameter as Query;
            if (Query != null && !string.IsNullOrEmpty(Query.SearchPhrase))
            {
                LoadData();
            }
        }
        public override void LoadData()
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
                            //Query query = new Query
                            //{
                            //    SearchPhrase = queryString,
                            //    PageIndex = pageIndex,
                            //    PageSize = pageSize
                            //};
                            Query.PageIndex = pageIndex;
                            Query.PageSize = pageSize;

                            var tracks = await DataService.GetTrackSearchResults(Query);
                            if (tracks != null)
                            {
                                foreach (var track in tracks)
                                {
                                    this.Items.Add(new GridPanelItemViewModel
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
