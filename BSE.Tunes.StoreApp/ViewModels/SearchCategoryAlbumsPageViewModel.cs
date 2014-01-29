using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Collections;
using BSE.Tunes.StoreApp.DataModel;
using BSE.Tunes.StoreApp.Interfaces;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchCategoryAlbumsPageViewModel : ViewModelBase, INavigationAware
    {
        #region FieldsPrivate
        private IDataService m_dataService;
        private INavigationService m_navigationService;
        private IResourceService m_resourceService;
        private IncrementalObservableCollection<DataItemViewModel> m_items;
        private ICommand m_selectCommand;
        private string m_pageTitle;
        private ICommand m_goBackCommand;
        #endregion

        #region Properties
        public ICommand GoBackCommand
        {
            get
            {
                return this.m_goBackCommand ??
                    (this.m_goBackCommand = new RelayCommand(this.m_navigationService.GoBack));
            }
        }
        public string PageTitle
        {
            get
            {
                return this.m_pageTitle;
            }
            set
            {
                this.m_pageTitle = value;
                RaisePropertyChanged("PageTitle");
            }
        }
        public IncrementalObservableCollection<DataItemViewModel> Items
        {
            get
            {
                return this.m_items;
            }
            private set
            {
                this.m_items = value;
                RaisePropertyChanged("Items");
            }
        }
        public ICommand SelectCommand
        {
            get
            {
                return this.m_selectCommand ??
                    (this.m_selectCommand = new RelayCommand<DataItemViewModel>(this.SelectItem));
            }
        }
        #endregion

        #region MethodsPublic
        public SearchCategoryAlbumsPageViewModel(IDataService dataService, INavigationService navigationService, IResourceService resourceService)
        {
            this.m_dataService = dataService;
            this.m_navigationService = navigationService;
            this.m_resourceService = resourceService;
        }
        public void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode)
        {
            string queryString = navigationParameter as string;
            if (string.IsNullOrEmpty(queryString) == false)
            {
                this.PageTitle = string.Format(CultureInfo.CurrentCulture,
                    this.m_resourceService.GetString("IDS_SearchAlbumsPage_PageTitle", "Results for {0}"), queryString);

                int maximumItems = 100;
                int pageIndex = 0;

                this.Items = new IncrementalObservableCollection<DataItemViewModel>(
                    (uint)maximumItems,
                    (uint count) =>
                    {
                        Func<Task<Windows.UI.Xaml.Data.LoadMoreItemsResult>> taskFunc = async () =>
                        {
                            int pageSize = (int)count;
                            Query query = new Query
                            {
                                SearchPhrase = queryString,
                                PageIndex = pageIndex,
                                PageSize = pageSize
                            };
                            
                            ObservableCollection<Album> albumsResults = await this.m_dataService.GetAlbumSearchResults(query);
                            if (albumsResults != null)
                            {
                                foreach (var album in albumsResults)
                                {
                                    this.Items.Add(new DataItemViewModel
                                    {
                                        Title = album.Title,
                                        Subtitle = album.Artist.Name,
                                        Image = album.Thumbnail,
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
        }
        public void OnNavigatedFrom(bool suspending)
        {
            if (this.Items != null)
            {
                this.Items = null;
            }
        }
        #endregion

        #region MethodsPrivate
        private void SelectItem(DataItemViewModel dataItem)
        {
            if (dataItem != null)
            {
                Album album = dataItem.Data as Album;
                if (album != null)
                {
                    this.m_navigationService.Navigate(typeof(AlbumDetailPage), album.Id);
                }
            }
        }
        #endregion
    }
}
