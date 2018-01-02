using BSE.Tunes.Data;
using BSE.Tunes.Data.Extensions;
using BSE.Tunes.StoreApp.Collections;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BSE.Tunes.StoreApp.Models;

namespace BSE.Tunes.StoreApp.ViewModels
{
	public class AlbumsPageViewModel : SelectableItemsBaseViewModel
	{
		#region FieldsPrivate
		private IncrementalObservableCollection<ListViewItemViewModel> m_albums;
        private ObservableCollection<int> m_filteredTrackIds;
        private ObservableCollection<MenuFlyoutItemViewModel> m_genres;
        private int m_tracksCount;
        private string m_headerText;
		private string m_pageHeaderText;
		private bool m_isGenreFlyoutOpen;
		private ICommand m_openGenreFlyoutCommand;
        private RelayCommand m_playRandomCommand;
        private Genre m_selectedGenre;

        #endregion

        #region Properties
        public ObservableCollection<int> FilteredTrackIds
        {
            get
            {
                return m_filteredTrackIds;
            }
            set
            {
                m_filteredTrackIds = value;
                RaisePropertyChanged(() => FilteredTrackIds);
            }
        }
        public int TracksCount
        {
            get
            {
                return m_tracksCount;
            }
            set
            {
                m_tracksCount = value;
                RaisePropertyChanged(() => TracksCount);
            }
        }
        public Genre SelectedGenre
		{
			get
			{
				return m_selectedGenre;
			}
			set
			{
				m_selectedGenre = value;
				RaisePropertyChanged(() => SelectedGenre);
			}
		}
		public bool IsGenreFlyoutOpen
		{
			get
			{
				return m_isGenreFlyoutOpen;
			}
			set
			{
				m_isGenreFlyoutOpen = value;
				RaisePropertyChanged(() => IsGenreFlyoutOpen);
			}
		}
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
				RaisePropertyChanged("Albums");
			}
		}
		public ObservableCollection<MenuFlyoutItemViewModel> Genres => m_genres ?? (m_genres = new ObservableCollection<MenuFlyoutItemViewModel>());
		public ICommand OpenGenreFlyoutCommand => m_openGenreFlyoutCommand ?? (m_openGenreFlyoutCommand = new RelayCommand(() =>
		{
			IsGenreFlyoutOpen = true;
		}));
        public RelayCommand PlayRandomCommand => m_playRandomCommand ?? (m_playRandomCommand = new RelayCommand(PlayRandom, CanPlayRandom));
        #endregion

        #region MethodsPublic
        public async override void LoadData()
        {
            SelectedGenre = new Genre
            {
                Name = ResourceService.GetString("AlbumsPage_FilterPanel_AllGenres-Value", "All")
            };
            var genres = await DataService?.GetGenres();
            if (genres != null)
            {
                var allGenres = new MenuFlyoutItemViewModel
                {
                    Text = SelectedGenre.Name,
                    Data = SelectedGenre
                };
                allGenres.ItemClicked += OnMenuItemViewModelClicked;
                Genres.Add(allGenres);

                foreach (var genre in genres)
                {
                    if (genre != null)
                    {
                        var menuItem = new MenuFlyoutItemViewModel
                        {
                            Text = genre.Name,
                            Data = genre
                        };
                        menuItem.ItemClicked += OnMenuItemViewModelClicked;
                        Genres.Add(menuItem);
                    }
                }
                
            }
            await LoadAlbums(null);
        }

        private async Task LoadAlbums(int? genreId)
        {
            this.Albums = null;
            int iNumberOfPlayableAlbums = await DataService?.GetNumberOfAlbumsByGenre(genreId);
            int pageNumber = 0;

            this.Albums = new IncrementalObservableCollection<ListViewItemViewModel>(
                (uint)iNumberOfPlayableAlbums,
                (uint count) =>
                {
                    Func<Task<Windows.UI.Xaml.Data.LoadMoreItemsResult>> taskFunc = async () =>
                    {
                        int pageSize = (int)count;
                        ObservableCollection<Album> albums = await DataService?.GetAlbumsByGenre(genreId, pageNumber, pageSize);
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
            ObservableCollection<int> trackIds = await DataService.GetTrackIdsByGenre(genreId);
            if (trackIds != null)
            {
                FilteredTrackIds = trackIds.ToRandomCollection();
                TracksCount = FilteredTrackIds?.Count ?? 0;
            }
            PlayRandomCommand.RaiseCanExecuteChanged();
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
		private void OnMenuItemViewModelClicked(object sender, EventArgs e)
		{
			IsGenreFlyoutOpen = false;
			SetGenre(sender as MenuFlyoutItemViewModel);
		}

		private async void SetGenre(MenuFlyoutItemViewModel menuItemViewModel)
		{
			SelectedGenre = menuItemViewModel.Data;
			await LoadAlbums(SelectedGenre.Id);
		}
        private bool CanPlayRandom()
        {
            return FilteredTrackIds?.Count > 0;
        }

        private void PlayRandom()
        {
            PlayerManager.PlayTracks(this.FilteredTrackIds, PlayerMode.Random);
        }
        #endregion
    }
}
