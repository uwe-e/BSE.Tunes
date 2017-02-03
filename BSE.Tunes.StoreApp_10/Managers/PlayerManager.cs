using BSE.Tunes.Data;
using BSE.Tunes.Data.Extensions;
using BSE.Tunes.Data.Collections;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using BSE.Tunes.StoreApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using BSE.Tunes.StoreApp.ViewModels;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;


namespace BSE.Tunes.StoreApp.Managers
{
    public class PlayerManager : IPlayerManager
    {
        #region FieldsPrivate
        private IDataService m_dataService;
        private IAuthenticationService m_accountService;
        private IDialogService m_dialogService;
        private IResourceService m_resourceService;
        #endregion

        #region Properties
        public IPlayerService PlayerService
        {
            get;
            private set;
        }
        public NavigableCollection<int> Playlist
        {
            get;set;
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
        public static PlayerManager Instance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IPlayerManager>() as PlayerManager;
            }
        }
        public PlayerManager(IDataService dataService, IAuthenticationService accountService, IPlayerService playerService, IDialogService dialogservice, IResourceService resourceService)
        {
            this.m_dataService = dataService;
            this.m_accountService = accountService;
            this.PlayerService = playerService;
            this.m_dialogService = dialogservice;
            this.m_resourceService = resourceService;
            Messenger.Default.Register<MediaStateChangedArgs>(this, args =>
            {
                switch (args.MediaState)
                {
                    case MediaState.Opened:
                        OnMediaOpened();
                        break;
                    case MediaState.Ended:
                        this.OnMediaEnded();
                        break;
                    case MediaState.NextRequested:
                        ExecuteNextTrack();
                        break;
                    case MediaState.PreviousRequested:
                        ExecutePreviousTrack();
                        break;
                    case MediaState.DownloadCompleted:
                        PrepareNextTrack();
                        break;
                }
            });
        }
        public async void ReplayPlayTracks()
        {
            int trackId = this.Playlist?.FirstOrDefault() ?? 0;
            if (trackId > 0)
            {
                Track track = await this.m_dataService.GetTrackById(trackId);
                if (track != null)
                {
                    await SetTrackAsync(track);
                }
            }
        }
        public void PlayTracks(ObservableCollection<int> trackIds, PlayerMode playerMode)
        {
            this.Playlist = trackIds?.ToNavigableCollection();
            PlayTrack(this.Playlist?.FirstOrDefault() ?? 0, playerMode);
        }
        public async void PlayTrack(int trackId, PlayerMode playerMode)
        {
            this.PlayerMode = playerMode;
            if (trackId > 0)
            {
                var track = await this.m_dataService.GetTrackById(trackId);
                if (track != null)
                {
                    await this.SetTrackAsync(track);
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
                catch (Exception exception)
                {
                    await this.m_dialogService.ShowAsync(exception.Message);
                }
            }
        }
        public bool CanExecuteNextTrack()
        {
			return this.Playlist?.CanMoveNext ?? false;
        }
        public async void ExecuteNextTrack()
        {
            if (this.CanExecuteNextTrack())
            {
                if (this.Playlist.MoveNext())
                {
                    var trackId = this.Playlist.Current;
                    if (trackId > 0)
                    {
                        Track track = await this.m_dataService.GetTrackById(trackId);
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
			return this.Playlist?.CanMovePrevious ?? false;
        }
        public async void ExecutePreviousTrack()
        {
            if (this.CanExecutePreviousTrack())
            {
                if (this.Playlist.MovePrevious())
                {
                    var trackId = this.Playlist.Current;
                    if (trackId > 0)
                    {
                        var track = await this.m_dataService.GetTrackById(trackId);
                        if (track != null)
                        {
                            await this.SetTrackAsync(track);
                        }
                    }
                }
            }
        }

        public async void PrepareNextTrack()
        {
            if (this.CanExecuteNextTrack())
            {
                var trackId = this.Playlist[Playlist.Index + 1];
                if (trackId > 0)
                {
                    var track = await this.m_dataService.GetTrackById(trackId);
                    if (track != null)
                    {
                        await PlayerService.PrepareTrack(track);
                        //await this.SetTrackAsync(track);
                    }
                }
            }
        }
        public bool CanExecutePlay()
        {
			return this.Playlist?.Count > 0;
        }
        public void Play()
        {
            this.PlayerService.Play();
        }
        public void Pause()
        {
            this.PlayerService.Pause();
        }
        #endregion

        #region MethodsPrivate
        private void OnMediaEnded()
        {
            if (this.PlayerMode != PlayerMode.None)
            {
                this.ExecuteNextTrack();
            }
        }
        private void OnMediaOpened()
        {
            try
            {
                PlayerService.CanExecuteNextTrack = this.CanExecuteNextTrack();
                PlayerService.CanExecutePreviousTrack = this.CanExecutePreviousTrack();

                string userName = "unknown";
                User user = SettingsService.Instance.User;
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    userName = user.UserName;
                }

                this.m_dataService.UpdateHistory(new History
                {
                    PlayMode = (Data.Audio.PlayerMode)this.PlayerMode,
                    AlbumId = this.CurrentTrack.Album.Id,
                    TrackId = this.CurrentTrack.Id,
                    UserName = userName,
                    PlayedAt = DateTime.Now
                });
            }
            catch { }
        }
        #endregion
    }
}
