using BSE.Tunes.Data;
using BSE.Tunes.Data.Audio;
using BSE.Tunes.Data.Extensions;
using BSE.Tunes.Data.Collections;
using BSE.Tunes.StoreApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using BSE.Tunes.StoreApp.ViewModels;
using BSE.Tunes.StoreApp.Messaging;
using System.Globalization;

namespace BSE.Tunes.StoreApp.Managers
{
    public class PlayerManager : IPlayerManager
    {
        #region FieldsPrivate
        private IDataService m_dataService;
        private IAccountService m_accountService;
        private IDialogService m_dialogService;
        private IResourceService m_resourceService;
        private NavigableCollection<Track> m_navigableTracks;
        private ObservableCollection<Track> m_tracks;
        #endregion

        #region Properties
        public IPlayerService PlayerService
        {
            get;
            private set;
        }
        public ObservableCollection<Track> Tracks
        {
            get
            {
                return this.m_tracks;
            }
            set
            {
                this.m_tracks = value;
            }
        }
        public Track CurrentTrack
        {
            get
            {
                return this.PlayerService.CurrentTrack;
            }
        }
        public TimeSpan Duration
        {
            get
            {
                return this.PlayerService.Duration;
            }
        }
        public TimeSpan Position
        {
            get
            {
                return this.PlayerService.Position;
            }
        }
        public PlayerMode PlayerMode
        {
            get;
            private set;
        }
        
        #endregion

        #region MethodsPublic
        public PlayerManager(IDataService dataService, IAccountService accountService, IPlayerService playerService, IDialogService dialogservice, IResourceService resourceService)
        {
            this.m_dataService = dataService;
            this.m_accountService = accountService;
            this.PlayerService = playerService;
            this.m_dialogService = dialogservice;
            this.m_resourceService = resourceService;
            Messenger.Default.Register<MediaOpenedMessage>(this, message =>
            {
                this.OnMediaOpened();
            });
            Messenger.Default.Register<MediaEndedMessage>(this, message =>
            {
                this.OnMediaEnded();
            });
            Messenger.Default.Register<MediaNextPressedMessage>(this, message =>
            {
                if (this.CanExecuteNextTrack())
                {
                    this.ExecuteNextTrack();
                }
            });
            Messenger.Default.Register<MediaPreviousPressedMessage>(this, message =>
            {
                if (this.CanExecutePreviousTrack())
                {
                    this.ExecutePreviousTrack();
                }
            });
        }
        public async void ReplayPlayTracks()
        {
            if (this.Tracks != null)
            {
                this.m_navigableTracks = this.Tracks.ToNavigableCollection();
                var track = this.m_navigableTracks.FirstOrDefault();
                if (track != null)
                {
                    track = await this.m_dataService.GetTrackById(track.Id);
                    if (track != null)
                    {
                        await SetTrackAsync(track);
                    }
                }
            }
        }

        public async void PlayTracks(ObservableCollection<Track> tracks, PlayerMode playerMode)
        {
            this.PlayerMode = playerMode;
            this.Tracks = tracks;
            if (this.Tracks != null)
            {
                this.m_navigableTracks = this.Tracks.ToNavigableCollection();
                var track = this.m_navigableTracks.FirstOrDefault();
                if (track != null)
                {
                    track = await this.m_dataService.GetTrackById(track.Id);
                    if (track != null)
                    {
                        await this.SetTrackAsync(track);
                    }
                }
            }
        }
        public async Task SetTrackAsync(Track track)
        {
            if (track != null)
            {
                try
                {
                    await this.PlayerService.SetTrackAsync(track);
                }
                catch (UnauthorizedAccessException)
                {
                    string message = string.Format(CultureInfo.CurrentCulture, this.m_resourceService.GetString("IDS_AudioDirectory_UnauthorizedAccessException"));
                    this.m_dialogService.ShowDialog(message);
                }
                catch (Exception exception)
                {
                    this.m_dialogService.ShowDialog(exception.Message);
                }
            }
        }
        public bool CanExecuteNextTrack()
        {
            return this.m_navigableTracks != null ? this.m_navigableTracks.CanMoveNext : false; ;
        }
        public async void ExecuteNextTrack()
        {
            if (this.CanExecuteNextTrack())
            {
                if (this.m_navigableTracks.MoveNext())
                {
                    Track track = this.m_navigableTracks.Current;
                    if (track != null)
                    {
                        track = await this.m_dataService.GetTrackById(track.Id);
                        if (track != null)
                        {
                            await this.SetTrackAsync(track);
                        }
                    }
                }
            }
        }
        public bool CanExecutePreviousTrack()
        {
            return this.m_navigableTracks != null ? this.m_navigableTracks.CanMovePrevious : false; ;
        }
        public async void ExecutePreviousTrack()
        {
            if (this.CanExecutePreviousTrack())
            {
                if (this.m_navigableTracks.MovePrevious())
                {
                    Track track = this.m_navigableTracks.Current;
                    if (track != null)
                    {
                        track = await this.m_dataService.GetTrackById(track.Id);
                        if (track != null)
                        {
                            await this.SetTrackAsync(track);
                        }
                    }
                }
            }
        }
        public bool CanExecutePlay()
        {
            return this.Tracks != null && this.Tracks.Count > 0;
        }
        public void Play()
        {
            this.PlayerService.Play();
        }
        public void Pause()
        {
            this.PlayerService.Pause();
        }
        public void Stop()
        {
            this.PlayerService.Stop();
        }
        #endregion

        #region MethodsPrivate
        private void OnMediaEnded()
        {
            if (this.PlayerMode != Data.Audio.PlayerMode.None)
            {
                if (this.CanExecuteNextTrack())
                {
                    this.ExecuteNextTrack();
                }
            }
        }
        private void OnMediaOpened()
        {
            try
            {
                this.PlayerService.CanExecuteNextTrack = this.CanExecuteNextTrack();
                this.PlayerService.CanExecutePreviousTrack = this.CanExecutePreviousTrack();

                string userName = "unknown";
                TunesUser user = this.m_accountService.User;
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    userName = user.UserName;
                }

                History history = new History
                {
                    PlayMode = this.PlayerMode,
                    AlbumId = this.CurrentTrack.Album.Id,
                    TrackId = this.CurrentTrack.Id,
                    UserName = userName,
                    PlayedAt = DateTime.Now
                };

                HistoryViewModel historyViewModel = new HistoryViewModel(this.m_dataService);
                historyViewModel.UpdateHistory(history);
            }
            catch { }
        }
        #endregion
    }
}
