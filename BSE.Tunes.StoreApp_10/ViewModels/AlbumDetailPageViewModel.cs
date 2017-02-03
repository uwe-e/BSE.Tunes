using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class AlbumDetailPageViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private Album m_album;
        private Uri m_coverSource;
        private RelayCommand m_playAllCommand;

        private PlayerManager m_playerManager;
        private ICommand m_playTrackCommand;
        #endregion

        #region Properties
        public Album Album
        {
            get
            {
                return this.m_album;
            }
            set
            {
                this.m_album = value;
                RaisePropertyChanged("Album");
            }
        }
        public Uri CoverSource
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
        public RelayCommand PlayAllCommand => m_playAllCommand ?? (m_playAllCommand = new RelayCommand(PlayAll, CanPlayAll));
        public ICommand PlayTrackCommand => m_playTrackCommand ?? (m_playTrackCommand = new RelayCommand<Track>(PlayTrack));

        
        #endregion

        #region MethodsPublic
        public AlbumDetailPageViewModel()
        {
            m_playerManager = PlayerManager.Instance;
        }
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Album album = parameter as Album;
            if (album != null)
            {
                this.Album = await DataService.GetAlbumById(album.Id);
                this.CoverSource = DataService.GetImage(album.AlbumId);
                this.PlayAllCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region MethodsPrivate
        private bool CanPlayAll()
        {
            return this.Album != null && this.Album.Tracks != null && this.Album?.Tracks?.Count() > 0;
        }
        private void PlayAll()
        {
            var tracks = new System.Collections.ObjectModel.ObservableCollection<Track>(this.Album.Tracks);
            if (tracks != null && tracks.Count() > 0)
            {
                this.PlayTracks(tracks);
            }
        }
        private void PlayTracks(System.Collections.ObjectModel.ObservableCollection<Track> tracks)
        {
            if (tracks != null)
            {
                var trackIds = tracks.Select(track => track.Id);
                if (trackIds != null)
                {
                    this.m_playerManager.PlayTracks(
                        new System.Collections.ObjectModel.ObservableCollection<int>(trackIds),
                        PlayerMode.CD);
                }
            }
        }
        private void PlayTrack(Track track)
        {
            this.m_playerManager.PlayTrack(track.Id, PlayerMode.Song);
        }
        #endregion
    }
}
