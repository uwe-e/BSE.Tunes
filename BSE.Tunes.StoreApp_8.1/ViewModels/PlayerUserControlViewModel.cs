using BSE.Tunes.Data;
using BSE.Tunes.Data.Collections;
using BSE.Tunes.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using BSE.Tunes.Data.Audio;
using GalaSoft.MvvmLight.Command;
using System.Globalization;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Managers;
using GalaSoft.MvvmLight.Messaging;
using BSE.Tunes.StoreApp.Messaging;
using BSE.Tunes.StoreApp.Extensions;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlayerUserControlViewModel : BasePlaylistableViewModel
    {
        #region FieldsPrivate
        private PlayerManager m_playerManager;
        private INavigationService m_navigationService;
        private bool m_bIsPlaying;
        private PlayerMode m_playerMode;
        private PlayerState m_playerState;
        private NavigableCollection<Track> m_filteredTracks;
        private ICommand m_selectCommand;
        private ICommand m_playCommand;
        private RelayCommand m_previousTrackCommand;
        private ICommand m_stopCommand;
        private RelayCommand m_nextTrackCommand;
        private double m_iProgressMaximumValue;
        private double m_iProgressValue;
        private double m_stepFrequency;
        private Track m_currentTrack;
        private string m_currentArtist;
        private string m_currentTitle;
        private string m_currentProgressTime;
        private string m_currentTrackDuration;
        private Uri m_coverSource;
        private DispatcherTimer m_progressTimer;
        #endregion

        #region Properties
        public bool IsPlaying
        {
            get { return this.m_bIsPlaying; }
            set
            {
                this.m_bIsPlaying = value;
                RaisePropertyChanged("IsPlaying");
            }
        }
        public PlayerMode PlayerMode
        {
            get
            {
                return this.m_playerMode;
            }
            set
            {
                this.m_playerMode = value;
                RaisePropertyChanged("PlayerMode");
            }
        }
        public PlayerState PlayerState
        {
            get
            {
                return this.m_playerState;
            }
            set
            {
                this.m_playerState = value;
                RaisePropertyChanged("PlayerState");
            }
        }
        public NavigableCollection<Track> FilteredTracks
        {
            get
            {
                return this.m_filteredTracks;
            }
            set
            {
                this.m_filteredTracks = value;
                RaisePropertyChanged("FilteredTracks");
            }
        }
        public Track CurrentTrack
        {
            get
            {
                return this.m_currentTrack;
            }
            set
            {
                Track oldValue = this.m_currentTrack;
                this.m_currentTrack = value;
                RaisePropertyChanged<Track>(() => this.CurrentTrack, oldValue, value, true);
            }
        }
        public double ProgressValue
        {
            get
            {
                return this.m_iProgressValue;
            }
            set
            {
                this.m_iProgressValue = value;
                RaisePropertyChanged("ProgressValue");
            }
        }

        public double ProgressMaximumValue
        {
            get
            {
                return this.m_iProgressMaximumValue;
            }
            set
            {
                this.m_iProgressMaximumValue = value;
                RaisePropertyChanged("ProgressMaximumValue");
            }
        }
        public double StepFrequency
        {
            get
            {
                return this.m_stepFrequency;
            }
            set
            {
                this.m_stepFrequency = value;
                RaisePropertyChanged("StepFrequency");
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
                Uri oldValue = this.m_coverSource;
                this.m_coverSource = value;
                RaisePropertyChanged<Uri>(() => this.CoverSource, oldValue, value, true);
            }
        }
        public string CurrentTitle
        {
            get
            {
                return this.m_currentTitle;
            }
            set
            {
                this.m_currentTitle = value;
                RaisePropertyChanged("CurrentTitle");
            }
        }
        public string CurrentArtist
        {
            get
            {
                return this.m_currentArtist;
            }
            set
            {
                this.m_currentArtist = value;
                RaisePropertyChanged("CurrentArtist");
            }
        }
        public string CurrentProgressTime
        {
            get
            {
                return this.m_currentProgressTime;
            }
            set
            {
                this.m_currentProgressTime = value;
                RaisePropertyChanged("CurrentProgressTime");
            }
        }
        public string CurrentTrackDuration
        {
            get
            {
                return this.m_currentTrackDuration;
            }
            set
            {
                this.m_currentTrackDuration = value;
                RaisePropertyChanged("CurrentTrackDuration");
            }
        }
        //public override ObservableCollection<MenuItemViewModel> MenuItemsPlaylist
        //{
        //	get;
        //	set;
        //}
        public ICommand SelectCommand
        {
            get
            {
                return this.m_selectCommand ??
                    (this.m_selectCommand = new RelayCommand(this.SelectItem));
            }
        }
        public ICommand PlayCommand
        {
            get
            {
                return this.m_playCommand ??
                    (this.m_playCommand = new RelayCommand<object>(vm => Play()));
            }
        }
        public ICommand StopCommand
        {
            get
            {
                return this.m_stopCommand ??
                    (this.m_stopCommand = new RelayCommand<object>(vm => this.m_playerManager.Stop()));
            }
        }
        public RelayCommand PreviousTrackCommand
        {
            get
            {
                return this.m_previousTrackCommand ??
                    (this.m_previousTrackCommand = new RelayCommand(this.m_playerManager.ExecutePreviousTrack, this.m_playerManager.CanExecutePreviousTrack));
            }
        }
        public RelayCommand NextTrackCommand
        {
            get
            {
                return this.m_nextTrackCommand ??
                    (this.m_nextTrackCommand = new RelayCommand(this.m_playerManager.ExecuteNextTrack, this.m_playerManager.CanExecuteNextTrack));
            }
        }
        #endregion

        #region MethodsPublic
        public PlayerUserControlViewModel(IDataService dataService, IAccountService accountService, IDialogService dialogService, IResourceService resourceService, PlayerManager playerManager, INavigationService navigationService, ICacheableBitmapService cacheableBitmapService)
            : base(dataService, accountService, dialogService, resourceService, cacheableBitmapService)
        {
            this.m_playerManager = playerManager;
            this.m_navigationService = navigationService;
            this.PlayerMode = Data.Audio.PlayerMode.Random;
            this.PlayerState = Data.Audio.PlayerState.Closed;
            Messenger.Default.Register<BSE.Tunes.Data.Audio.PlayerState>(this, playerState =>
            {
                this.OnPlayerStateChanged(playerState);
            });
            Messenger.Default.Register<BSE.Tunes.StoreApp.Messaging.MediaOpenedMessage>(this, message =>
            {
                this.OnMediaOpened();
            });
            Messenger.Default.Register<BSE.Tunes.StoreApp.Messaging.MediaEndedMessage>(this, message =>
            {
                this.OnMediaEnded();
            });
            Messenger.Default.Register<TrackMessage>(this, message =>
            {
                Track track = message.Track;
                if (track != null)
                {
                    this.OnInitializeView(track);
                }
            });
            Messenger.Default.Register<PlaylistChangeMessage>(this, message =>
            {
                this.LoadPlaylists();
            });
            this.MenuItemsPlaylist = new ObservableCollection<MenuItemViewModel>();
            this.LoadPlaylists();
        }
        public override void ResetData()
        {
            base.ResetData();

        }
        #endregion

        #region MethodsProtected
        protected override void AddTracksToPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                playlist.Entries.Clear();
                playlist.Entries.Add(new PlaylistEntry
                {
                    PlaylistId = playlist.Id,
                    TrackId = this.CurrentTrack.Id,
                    Guid = Guid.NewGuid()
                });
                base.AddTracksToPlaylist(playlist);
            }
        }
        #endregion

        #region MethodsPrivate
        private void SelectItem()
        {
            if (this.CurrentTrack != null && this.CurrentTrack.Album != null)
            {
                this.m_navigationService.Navigate(typeof(BSE.Tunes.StoreApp.Views.AlbumDetailPage), this.CurrentTrack.Album.Id);
            }
        }
        private void Play()
        {
            try
            {
                switch (this.PlayerState)
                {
                    case Data.Audio.PlayerState.Stopped:
                    case Data.Audio.PlayerState.Closed:
                        if (this.m_playerManager.CanExecutePlay())
                        {
                            this.m_playerManager.ReplayPlayTracks();
                        }
                        break;
                    case Data.Audio.PlayerState.Playing:
                        this.m_playerManager.Pause();
                        break;
                    case Data.Audio.PlayerState.Paused:
                        this.m_playerManager.Play();
                        break;
                }
            }
            catch (Exception exception)
            {
                this.DialogService.ShowDialog(exception.Message);
            }
        }
        private void OnInitializeView(Track track)
        {
            if (track != null)
            {
                Messenger.Default.Send<Track>(track);
                this.CurrentTrack = track;
                this.CurrentTitle = track.Name;
                this.CurrentArtist = track.Album.Artist.Name;
                //this.Cover = track.Album.Cover;
                this.CoverSource = this.DataService.GetImage(track.Album.AlbumId);
            }
        }
        private void OnMediaOpened()
        {
            this.ProgressValue = 0;
            this.CurrentTrack = this.m_playerManager.CurrentTrack;
            this.CurrentTitle = this.CurrentTrack.Name;
            this.CurrentArtist = this.CurrentTrack.Album.Artist.Name;
            this.CurrentTrackDuration = this.CurrentTrack.Duration.ToString();
            this.CurrentProgressTime = TimeSpan.FromMinutes(0).ToString();

            Uri coverSource = this.DataService.GetImage(this.CurrentTrack.Album.AlbumId);
            if (coverSource != null && !coverSource.Equals(this.CoverSource))
            {
                this.CoverSource = coverSource;
            }

            this.ProgressMaximumValue = this.m_playerManager.Duration.TotalSeconds;
            this.StepFrequency = this.SliderFrequency(this.m_playerManager.Duration);

            this.m_progressTimer = new DispatcherTimer();
            this.m_progressTimer.Interval = TimeSpan.FromSeconds(this.StepFrequency);
            this.m_progressTimer.Tick += OnProgressTimerTick;
            this.m_progressTimer.Start();
        }
        private void OnMediaEnded()
        {
            this.m_progressTimer.Stop();
            this.m_progressTimer.Tick -= OnProgressTimerTick;
            this.ProgressMaximumValue = 100;
            this.ProgressValue = 0;
        }

        private void OnMediaFailed(object exceptionRoutedEventArgs)
        {
        }

        private void OnPlayerStateChanged(PlayerState playerState)
        {
            this.PlayerState = playerState;
            this.IsPlaying = false;
            switch (this.PlayerState)
            {
                case Data.Audio.PlayerState.Stopped:
                case Data.Audio.PlayerState.Paused:
                    this.m_progressTimer.Stop();
                    break;
                case PlayerState.Playing:
                    this.IsPlaying = true;
                    this.m_progressTimer.Start();
                    this.PreviousTrackCommand.RaiseCanExecuteChanged();
                    this.NextTrackCommand.RaiseCanExecuteChanged();
                    break;
            }
        }
        private void OnProgressTimerTick(object sender, object e)
        {
            this.ProgressValue = this.m_playerManager.Position.TotalSeconds;
            TimeSpan position = new TimeSpan(0, this.m_playerManager.Position.Hours, this.m_playerManager.Position.Minutes, this.m_playerManager.Position.Seconds);
            this.CurrentProgressTime = position.ToString();
        }
        private double SliderFrequency(TimeSpan timeSpan)
        {
            double stepfrequency = -1;

            double absvalue = (int)Math.Round(timeSpan.TotalSeconds, MidpointRounding.AwayFromZero);
            stepfrequency = (int)(Math.Round(absvalue / 100));

            if (timeSpan.TotalMinutes >= 10 && timeSpan.TotalMinutes < 30)
            {
                stepfrequency = 10;
            }
            else if (timeSpan.TotalMinutes >= 30 && timeSpan.TotalMinutes < 60)
            {
                stepfrequency = 30;
            }
            else if (timeSpan.TotalHours >= 1)
            {
                stepfrequency = 60;
            }

            if (stepfrequency == 0) stepfrequency += 1;

            if (stepfrequency == 1)
            {
                stepfrequency = absvalue / 100;
            }

            return stepfrequency;
        }
        #endregion
    }
}