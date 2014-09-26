using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Collections;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// try-catch (C# Reference)
    /// http://msdn.microsoft.com/en-us/library/vstudio/0yd65esw.aspx
    /// </remarks>
    public class AlbumsPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IDataService m_dataService;
        private INavigationService m_navigationService;
        private IDialogService m_dialogService;
        private IResourceService m_resourceService;
        private IncrementalObservableCollection<AlbumViewModel> m_albums;
        private ObservableCollection<SortOrderViewModel> m_sortOrders;
        private SortOrderViewModel m_selectedSortOrder;
        private ICommand m_sortOrderSelectionChangedCommand;
        private ICommand m_selectCommand;
        private ICommand m_goBackCommand;
        private bool m_queryChanged;
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
        public IncrementalObservableCollection<AlbumViewModel> Albums
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
        public ICommand SelectCommand
        {
            get
            {
                return this.m_selectCommand ??
                    (this.m_selectCommand = new GalaSoft.MvvmLight.Command.RelayCommand<AlbumViewModel>(this.SelectItem));
            }
        }
        public ObservableCollection<SortOrderViewModel> SortOrders
        {
            get
            {
                return this.m_sortOrders;
            }
            private set
            {
                this.m_sortOrders = value;
                RaisePropertyChanged("SortOrders");
            }
        }
        public ICommand SortOrderSelectionChangedCommand
        {
            get
            {
                return this.m_sortOrderSelectionChangedCommand
                    ?? (this.m_sortOrderSelectionChangedCommand = new GalaSoft.MvvmLight.Command.RelayCommand(this.ExecuteSortOrderSelection));
            }
        }
        public SortOrderViewModel SelectedSortOrder
        {
            get
            {
                return this.m_selectedSortOrder;
            }
            set
            {
                this.m_selectedSortOrder = value;
                RaisePropertyChanged("SelectedSortOrder");
            }
        }
        #endregion

        #region MethodsPublic
        public AlbumsPageViewModel(IDataService dataService, INavigationService navigationService, IDialogService dialogService, IResourceService resourceService)
        {
            this.m_dataService = dataService;
            this.m_navigationService = navigationService;
            this.m_dialogService = dialogService;

            this.m_resourceService = resourceService;
            this.Load();
        }
        public override void ResetData()
        {
            base.ResetData();
            this.Load();
        }
        #endregion

        #region MethodsPrivate
        private async void Load()
        {
            this.LoadSortOrders();
            Task taskAlbums = this.LoadAlbums();
            Task allTasks = Task.WhenAll(taskAlbums);
            try
            {
                await allTasks;
            }
            catch (Exception exception)
            {
                this.m_dialogService.ShowDialog(exception.Message);
            }
        }
        private void LoadSortOrders()
        {
            this.SortOrders = new ObservableCollection<SortOrderViewModel>();
            this.SortOrders.Add(new SortOrderViewModel(
                new SortOrder
                {
                    Id = 0,
                    Name = this.m_resourceService.GetString("IDS_Artists", "Artists"),
                }));
            this.SortOrders.Add(new SortOrderViewModel(
                new SortOrder
                {
                    Id = 1,
                    Name = this.m_resourceService.GetString("IDS_Albums", "Albums")
                }));

            this.SelectedSortOrder = this.SortOrders[0];
        }
        private async Task LoadAlbums()
        {
            this.Albums = null;
            int iNumberOfPlayableAlbums = await this.m_dataService.GetNumberOfPlayableAlbums();
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
                        ObservableCollection<Album> albums = await this.m_dataService.GetAlbums(query);
                        if (albums != null)
                        {
                            foreach (var album in albums)
                            {
                                this.Albums.Add(new AlbumViewModel(this.m_dataService, album));
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

            if (this.m_queryChanged == true)
            {
                await this.Albums.LoadMoreItemsAsync();
                this.m_queryChanged = false;
            }
        }
        private Query GetQuery()
        {
            return new Query
            {
                SortByCondition = this.SelectedSortOrder.SortOrder
            };
        }
        private void SelectItem(AlbumViewModel albumViewmodel)
        {
            if (albumViewmodel != null)
            {
                this.m_navigationService.Navigate(typeof(AlbumDetailPage), typeof(MasterPage), albumViewmodel.Album.Id);
            }
        }
        private async void ExecuteSortOrderSelection()
        {
            SortOrderViewModel sortOrderViewModel = this.SelectedSortOrder;
            if (sortOrderViewModel != null)
            {
                this.m_queryChanged = true;
                await LoadAlbums();
            }
        }
        #endregion
    }
}