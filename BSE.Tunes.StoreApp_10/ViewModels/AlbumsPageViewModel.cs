using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Collections;
using BSE.Tunes.StoreApp.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class AlbumsPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IncrementalObservableCollection<AlbumViewModel> m_albums;
        #endregion

        #region Properties
        public IncrementalObservableCollection<AlbumViewModel> Albums
        {
            //get; set;
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
        public AlbumsPageViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LoadData();
            }
        }
        #endregion

        #region MethodsPrivate
        private async void LoadData()
        {
            await LoadAlbums();
        }

        private async Task LoadAlbums()
        {
            this.Albums = null;
            int iNumberOfPlayableAlbums = await DataService?.GetNumberOfPlayableAlbums();
            int pageNumber = 0;

            this.Albums = new IncrementalObservableCollection<AlbumViewModel>(
                (uint)iNumberOfPlayableAlbums,
                (uint count) =>
                {
                    Func<Task<Windows.UI.Xaml.Data.LoadMoreItemsResult>> taskFunc = async () =>
                    {
                        int pageSize = (int)count;

                        Query query = GetQuery();
                        query.PageIndex = pageNumber;
                        query.PageSize = pageSize;
                        ObservableCollection<Album> albums = await DataService?.GetAlbums(query);
                        if (albums != null)
                        {
                            foreach (var album in albums)
                            {
                                this.Albums.Add(new AlbumViewModel(album));
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

            //if (this.m_queryChanged == true)
            //{
               // await this.Albums.LoadMoreItemsAsync();
            //    this.m_queryChanged = false;
            //}
        }

        private Query GetQuery()
        {
            //return new Query
            //{
            //    SortByCondition = this.SelectedSortOrder.SortOrder
            //};
            return new Query();
        }

        #endregion
    }
}
