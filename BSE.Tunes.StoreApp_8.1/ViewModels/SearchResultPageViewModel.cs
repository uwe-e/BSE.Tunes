using BSE.Tunes.Data;
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
    public class SearchResultPageViewModel : ViewModelBase, INavigationAware
    {
        #region FieldsPrivate
        private IDataService m_dataService;
        private INavigationService m_navigationService;
        private IResourceService m_resourceService;
		private IDialogService m_dialogservice;
        private ObservableCollection<DataGroupViewModel> m_searchResultGroups;
        private ICommand m_groupHeaderClickCommand;
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
        public ObservableCollection<DataGroupViewModel> SearchResultGroups
        {
            get
            {
                return this.m_searchResultGroups ??
                    (this.m_searchResultGroups = new ObservableCollection<DataGroupViewModel>());
            }
        }
        public ICommand SearchCategoryCommand
        {
            get
            {
                return this.m_groupHeaderClickCommand ??
                    (this.m_groupHeaderClickCommand = new RelayCommand<SearchCategory>(this.SearchCategory));
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
        public SearchResultPageViewModel(IDataService dataService, INavigationService navigationService, IResourceService resourceService, IDialogService dialogService)
        {
            this.m_dataService = dataService;
            this.m_navigationService = navigationService;
            this.m_resourceService = resourceService;
			this.m_dialogservice = dialogService;
        }
        public async void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode)
        {
            string queryString = navigationParameter as string;
            if (string.IsNullOrEmpty(queryString) == false)
            {
                this.PageTitle = string.Format(CultureInfo.CurrentCulture,
                    this.m_resourceService.GetString("IDS_SearchPane_PageTitle", "Results for {0}"), queryString);
                try
                {
                    Query query = new Query
                    {
                        SearchPhrase = queryString,
                        PageSize = 9
                    };
                    var searchResult = await this.m_dataService.GetSearchResults(query);
                    if (searchResult != null)
                    {
                        Album[] albums = searchResult.Albums;
                        if (albums != null && albums.Count() > 0)
                        {
                            DataGroupViewModel albumGroup = new DataGroupViewModel
                            {
                                Title = this.m_resourceService.GetString("IDS_SearchPane_GroupTitleAlbums", "Albums"),
                                Data = new SearchCategory
                                {
                                    Mode = DataModel.FilterMode.Albbums,
                                    Query = queryString
                                }
                            };
                            foreach (var album in albums)
                            {
                                if (album != null)
                                {
                                    DataItemViewModel dataItem = new DataItemViewModel();
                                    dataItem.Title = album.Title;
									dataItem.ImageSource = this.m_dataService.GetImage(album.AlbumId, true);
                                    dataItem.Subtitle = album.Artist.Name;
                                    dataItem.Data = album;
                                    albumGroup.Items.Add(dataItem);
                                }
                            }
                            this.SearchResultGroups.Add(albumGroup);
                        }
                        Track[] tracks = searchResult.Tracks;
                        if (tracks != null && tracks.Count() > 0)
                        {
                            DataGroupViewModel trackGroup = new DataGroupViewModel
                            {
                                Title = this.m_resourceService.GetString("IDS_SearchPane_GroupTitleTracks", "Tracks"),
                                Data = new SearchCategory
                                {
                                    Mode = DataModel.FilterMode.Tracks,
                                    Query = queryString
                                }
                            };
                            foreach (var track in tracks)
                            {
                                if (track != null)
                                {
                                    DataItemViewModel dataItem = new DataItemViewModel();
                                    dataItem.Title = track.Name;
									dataItem.ImageSource = this.m_dataService.GetImage(track.Album.AlbumId, true);
                                    dataItem.Subtitle = track.Album.Artist.Name;
                                    dataItem.Data = track;
                                    trackGroup.Items.Add(dataItem);
                                }
                            }
                            this.SearchResultGroups.Add(trackGroup);
                        }
                    }
                }
                catch (AggregateException aggregateException)
                {
					string errorMessage = string.Empty;
                    foreach (var innerException in aggregateException.Flatten().InnerExceptions)
                    {
						if (innerException != null && !string.IsNullOrEmpty(innerException.Message))
						{
							errorMessage += innerException.Message + Environment.NewLine;
						}
                    }
					if (!string.IsNullOrEmpty(errorMessage))
					{
						this.m_dialogservice.ShowDialog(errorMessage);
					}

                }
            }
        }
        public void OnNavigatedFrom(bool suspending)
        {
            if (this.SearchResultGroups != null)
            {
                this.SearchResultGroups.Clear();
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
                    this.m_navigationService.Navigate(typeof(AlbumDetailPage), typeof(MasterPage), album.Id);
                }
                Track track = dataItem.Data as Track;
                if (track != null)
                {
                    this.m_navigationService.Navigate(typeof(AlbumDetailPage), typeof(MasterPage), track.Album.Id);
                }
            }
        }
        private void SearchCategory(SearchCategory searchCategory)
        {
            if (searchCategory != null)
            {
                if (searchCategory.Mode == DataModel.FilterMode.Tracks)
                {
                    this.m_navigationService.Navigate(typeof(SearchCategoryTracksPage), typeof(MasterPage), searchCategory.Query);
                }
                if (searchCategory.Mode == DataModel.FilterMode.Albbums)
                {
                    this.m_navigationService.Navigate(typeof(SearchCategoryAlbumsPage), typeof(MasterPage), searchCategory.Query);
                }
            }
        }
        #endregion
    }
}
