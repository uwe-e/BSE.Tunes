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
		private NavigableCollection<int> m_navigableTrackIds;
		private ObservableCollection<int> m_trackIds;
        #endregion

        #region Properties
        public IPlayerService PlayerService
        {
            get;
            private set;
        }
		public ObservableCollection<int> TrackIds
		{
			get
			{
				return this.m_trackIds;
			}
			set
			{
				this.m_trackIds = value;
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
            //Messenger.Default.Register<MediaOpenedMessage>(this, message =>
            //{
            //    this.OnMediaOpened();
            //});
            //Messenger.Default.Register<MediaEndedMessage>(this, message =>
            //{
            //    this.OnMediaEnded();
            //});
            //Messenger.Default.Register<MediaNextPressedMessage>(this, message =>
            //{
            //    if (this.CanExecuteNextTrack())
            //    {
            //        this.ExecuteNextTrack();
            //    }
            //});
            //Messenger.Default.Register<MediaPreviousPressedMessage>(this, message =>
            //{
            //    if (this.CanExecutePreviousTrack())
            //    {
            //        this.ExecutePreviousTrack();
            //    }
            //});
        }
        public async void ReplayPlayTracks()
        {
            if (this.TrackIds != null)
            {
                this.m_navigableTrackIds = this.TrackIds.ToNavigableCollection();
                var trackId = this.m_navigableTrackIds.FirstOrDefault();
                if (trackId > 0)
                {
                    Track track = await this.m_dataService.GetTrackById(trackId);
                    if (track != null)
                    {
                        await SetTrackAsync(track);
                    }
                }
            }
        }
		public async void PlayTracks(ObservableCollection<int> trackIds, PlayerMode playerMode)
		{
			//this.PlayerMode = playerMode;
			//this.TrackIds = trackIds;
			//if (this.TrackIds != null)
			//{
			//	this.m_navigableTrackIds = this.TrackIds.ToNavigableCollection();
			//	var trackId = this.m_navigableTrackIds.FirstOrDefault();
			//	if (trackId > 0)
			//	{
			//		var track = await this.m_dataService.GetTrackById(trackId);
			//		if (track != null)
			//		{
			//			await this.SetTrackAsync(track);
			//		}
			//	}
			//}
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
			return this.m_navigableTrackIds != null ? this.m_navigableTrackIds.CanMoveNext : false;
        }
        public async void ExecuteNextTrack()
        {
    //        if (this.CanExecuteNextTrack())
    //        {
				//if (this.m_navigableTrackIds.MoveNext())
				//{
				//	var trackId = this.m_navigableTrackIds.Current;
				//	if (trackId > 0)
				//	{
				//		Track track = await this.m_dataService.GetTrackById(trackId);
				//		if (track != null)
				//		{
				//			await this.SetTrackAsync(track);
				//		}
				//	}
				//}
    //        }
        }
        public bool CanExecutePreviousTrack()
        {
			return this.m_navigableTrackIds != null ? this.m_navigableTrackIds.CanMovePrevious : false;
        }
        public async void ExecutePreviousTrack()
        {
    //        if (this.CanExecutePreviousTrack())
    //        {
				//if (this.m_navigableTrackIds.MovePrevious())
				//{
				//	var trackId = this.m_navigableTrackIds.Current;
				//	if (trackId > 0)
				//	{
				//		var track = await this.m_dataService.GetTrackById(trackId);
				//		if (track != null)
				//		{
				//			await this.SetTrackAsync(track);
				//		}
				//	}
				//}
    //        }
        }
        public bool CanExecutePlay()
        {
			return this.TrackIds != null && this.TrackIds.Count > 0;
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
                //this.PlayerService.CanExecuteNextTrack = this.CanExecuteNextTrack();
                //this.PlayerService.CanExecutePreviousTrack = this.CanExecutePreviousTrack();

                //string userName = "unknown";
                //TunesUser user = this.m_accountService.User;
                //if (user != null && !string.IsNullOrEmpty(user.UserName))
                //{
                //    userName = user.UserName;
                //}

                //History history = new History
                //{
                //    PlayMode = this.PlayerMode,
                //    AlbumId = this.CurrentTrack.Album.Id,
                //    TrackId = this.CurrentTrack.Id,
                //    UserName = userName,
                //    PlayedAt = DateTime.Now
                //};

                //HistoryViewModel historyViewModel = new HistoryViewModel(this.m_dataService);
                //historyViewModel.UpdateHistory(history);
            }
            catch { }
        }
        #endregion
    }
}
