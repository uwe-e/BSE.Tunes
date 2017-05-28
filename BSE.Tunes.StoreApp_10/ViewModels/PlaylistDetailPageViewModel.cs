using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Collections.Specialized;
using Template10.Services.NavigationService;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using GalaSoft.MvvmLight.Messaging;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistDetailPageViewModel : PlaylistBaseViewModel
    {
        #region FieldsPrivate
        private Playlist m_playlist;
        private BitmapSource m_coverSource;
        private string m_subTitle;
        private ICommand m_showAlbumCommand;
        #endregion

        #region Properties
        public Playlist Playlist
        {
            get
            {
                return this.m_playlist;
            }
            set
            {
                this.m_playlist = value;
                RaisePropertyChanged("Playlist");
            }
        }
        public BitmapSource CoverSource
        {
            get
            {
                return this.m_coverSource;
            }
            set
            {
                this.m_coverSource = value;
                RaisePropertyChanged("CoverSource");
            }
        }
        public string InfoSubTitle
        {
            get
            {
                return this.m_subTitle;
            }
            set
            {
                this.m_subTitle = value;
                RaisePropertyChanged("InfoSubTitle");
            }
        }
        public ICommand ShowAlbumCommand => m_showAlbumCommand ?? (m_showAlbumCommand = new RelayCommand<ListViewItemViewModel>(ShowAlbum));
        #endregion

        #region MethodsPublic
        public PlaylistDetailPageViewModel()
        {
            Messenger.Default.Register<PlaylistChangedArgs>(this, args =>
            {
                PlaylistEntriesChangedArgs playlistEntriesChanged = args as PlaylistEntriesChangedArgs;
                if (playlistEntriesChanged != null)
                {
                    LoadData(args.Playlist);
                }
            });
        }
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            LoadData(parameter as Playlist);
            await base.OnNavigatedToAsync(parameter, mode, state);
            
        }

        public async override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            await base.OnNavigatingFromAsync(args);
        }

        public override bool CanPlayAll()
        {
            return this.Playlist?.Entries != null && this.Playlist.Entries.Count() > 0;
        }
        public override void PlayAll()
        {
            var entryIds = this.Playlist.Entries.Select(entry => entry.TrackId);
            if (entryIds != null)
            {
                PlayerManager.PlayTracks(
                    new System.Collections.ObjectModel.ObservableCollection<int>(entryIds),
                    PlayerMode.Playlist);
            }
        }
        public override void PlayTrack(ListViewItemViewModel item)
        {
            PlayerManager.PlayTrack(((PlaylistEntry)item.Data).TrackId, PlayerMode.Song);
        }
        public async override void DeleteSelectedItems()
        {
            if (SelectedItems?.Count > 0)
            {
                var list = SelectedItems.ToList(); ;
                foreach (var item in list)
                {
                    Items.Remove((ListViewItemViewModel)item);
                }
                Playlist.Entries.Clear();
                foreach (var entry in Items.Select(itm => itm.Data).Cast<PlaylistEntry>())
                {
                    Playlist.Entries.Add(entry);
                }
                await DataService.UpdatePlaylistEntries(Playlist);
                ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                await cacheableBitmapService.RemoveCache(Playlist.Guid.ToString());
                Messenger.Default.Send<PlaylistChangedArgs>(new PlaylistEntriesChangedArgs(Playlist));
            }
        }
        #endregion

        #region MethodsProtected
        protected override void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnSelectedItemsCollectionChanged(sender, e);
            AllItemsSelected = Items.OrderBy(itm => ((PlaylistEntry)itm.Data).SortOrder).SequenceEqual(
                SelectedItems.Cast<ListViewItemViewModel>().OrderBy(itm => ((PlaylistEntry)itm.Data).SortOrder));
            AllItemsSelectable = HasSelectedItems & !AllItemsSelected;
        }
        #endregion

        #region MethodsPrivate
        private async void LoadData(Playlist playlist)
        {
            Items.Clear();
            if (playlist != null)
            {
                User user = SettingsService.Instance.User;
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    try
                    {
                        Collection<Uri> imageUris = null;
                        ICacheableBitmapService cacheableBitmapService = CacheableBitmapService.Instance;
                        Playlist = await this.DataService.GetPlaylistById(playlist.Id, user.UserName);
                        if (this.Playlist != null)
                        {                            
                            foreach (var entry in this.Playlist.Entries?.OrderBy(pe => pe.SortOrder))
                            {
                                if (entry != null)
                                {
                                    Items.Add(new ListViewItemViewModel { Data = entry });
                                    if (imageUris == null)
                                    {
                                        imageUris = new Collection<Uri>();
                                    }
                                    imageUris.Add(this.DataService.GetImage(entry.AlbumId));
                                }
                            }
                            if (imageUris != null)
                            {
                                this.CoverSource = await cacheableBitmapService.GetBitmapSource(
                                    new ObservableCollection<Uri>(imageUris.Take(4)),
                                    this.Playlist.Guid.ToString(),
                                    500);
                            }
                            this.InfoSubTitle = FormatNumberOfEntriesString(Playlist);
                        }
                    }
                    finally
                    {
                        this.PlayAllCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }
        private string FormatNumberOfEntriesString(Playlist playlist)
        {
            int numberOfEntries = 0;
            if (playlist != null)
            {
                numberOfEntries = playlist.Entries?.Count ?? 0;
            }
            return string.Format(CultureInfo.CurrentUICulture, "{0} {1}", numberOfEntries, ResourceService.GetString("PlaylistItem_PartNumberOfEntries", "Songs"));
        }
        private void ShowAlbum(ListViewItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), ((Track)((PlaylistEntry)item.Data).Track).Album);
        }
        #endregion

    }
}
