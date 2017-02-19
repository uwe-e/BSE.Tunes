using BSE.Tunes.StoreApp.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using System.Collections.Specialized;
using Template10.Services.NavigationService;
using GalaSoft.MvvmLight.Command;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.Data;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistBaseViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private bool m_hasSelectedItems;
        private bool m_isCommandBarVisible;
        private ObservableCollection<object> m_selectedItems;
        private RelayCommand m_playSelectedItemsCommand;
        private ICommand m_openPlaylistFlyoutCommand;
        private bool m_isPlaylistFlyoutOpen;
        #endregion

        #region Properties
        public bool IsPlaylistFlyoutOpen
        {
            get
            {
                return m_isPlaylistFlyoutOpen;
            }
            set
            {
                m_isPlaylistFlyoutOpen = value;
                RaisePropertyChanged("IsPlaylistFlyoutOpen");
            }
        }
        /// <summary>
        /// Gets or sets an value indication whether the tracklist has at the minimum one selected item.
        /// </summary>
        public virtual bool HasSelectedItems
        {
            get
            {
                return this.m_hasSelectedItems;
            }
            set
            {
                this.m_hasSelectedItems = value;
                RaisePropertyChanged("HasSelectedItems");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the the toolbar for the playlist related content is open.
        /// </summary>
        /// <remarks>Because it´s a twoway bound property in xaml the <see cref="HasSelectedItems"/> property can not used.</remarks>
        public virtual bool IsCommandBarVisible
        {
            get
            {
                return this.m_isCommandBarVisible;
            }
            set
            {
                this.m_isCommandBarVisible = value;
                RaisePropertyChanged("IsCommandBarVisible");
            }
        }
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return this.m_selectedItems;
            }
            set
            {
                this.m_selectedItems = value;
                RaisePropertyChanged("SelectedItems");
            }
        }
        public virtual ObservableCollection<MenuItemViewModel> MenuItemsPlaylist
        {
            get;
            set;
        }
        public RelayCommand PlaySelectedItemsCommand => m_playSelectedItemsCommand ?? (m_playSelectedItemsCommand = new RelayCommand(PlaySelectedItems, CanPlaySelectedItems));
        public ICommand OpenPlaylistFlyoutCommand => m_openPlaylistFlyoutCommand ?? (m_openPlaylistFlyoutCommand = new RelayCommand(OpenPlaylistFlyout));

        
        #endregion

        #region MethodsPublic
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (this.SelectedItems == null)
            {
                this.SelectedItems = new ObservableCollection<object>();
                this.SelectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
            }
            return Task.CompletedTask;
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (this.SelectedItems != null)
            {
                this.SelectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
                this.SelectedItems = null;
            }
            this.HasSelectedItems = false;
            this.IsCommandBarVisible = false;
            return base.OnNavigatedFromAsync(state, suspending);
        }
        #endregion

        #region MethodsProtected
        protected virtual void PlaySelectedItems()
        {
        }
        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectedItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.HasSelectedItems = this.SelectedItems.Count > 0;
            this.IsCommandBarVisible = this.HasSelectedItems;
            this.PlaySelectedItemsCommand.RaiseCanExecuteChanged();
            //this.ClearSelectionCommand.RaiseCanExecuteChanged();
        }
        protected virtual async void CreatePlaylistMenu()
        {
            if (this.MenuItemsPlaylist == null)
            {
                this.MenuItemsPlaylist = new ObservableCollection<MenuItemViewModel>();
            }
            this.MenuItemsPlaylist.Clear();

            User user = SettingsService.Instance.User;
            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                var playlists = await this.DataService.GetPlaylistsByUserName(user.UserName);
                if (playlists != null)
                {
                    this.CreateMenuItems(playlists);
                }
            }
            this.MenuItemsPlaylist.Insert(0, new MenuItemViewModel
            {
                IsSeparator = true
            });
            this.MenuItemsPlaylist.Insert(0,
                new MenuItemViewModel
                {
                    Text = this.ResourceService.GetString("FlyoutMenuItem_AddNewPlaylist", "New Playlist")
                });
        }
        protected virtual void CreateMenuItems(ObservableCollection<Playlist> playlists)
        {
            if (playlists != null)
            {
                foreach (var playlist in playlists)
                {
                    if (playlist != null)
                    {
                        this.MenuItemsPlaylist.Add(new PlaylistMenuItemViewModel { Text = playlist.Name, Playlist = playlist });
                    }
                }
            }
        }
        #endregion

        #region MethodsPrivate
        private bool CanPlaySelectedItems()
        {
            return this.SelectedItems?.Count > 0;
        }

        private void OpenPlaylistFlyout()
        {
            IsPlaylistFlyoutOpen = true;
        }
        #endregion
    }
}
