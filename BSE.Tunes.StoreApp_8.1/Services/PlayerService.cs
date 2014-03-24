using BSE.Tunes.Data;
using BSE.Tunes.Data.Audio;
using BSE.Tunes.Data.Collections;
using BSE.Tunes.Data.Extensions;
using BSE.Tunes.StoreApp.Extensions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Media.Core;
using System.IO;

namespace BSE.Tunes.StoreApp.Services
{
	public class PlayerService : IPlayerService
	{
		#region Attached
		/// <summary>
		/// Provides the data binding and property change notification of the <b>RegisterAsMediaService</b> attached property.
		/// </summary>
		public static readonly DependencyProperty RegisterAsMediaServiceProperty =
			DependencyProperty.RegisterAttached(
			"RegisterAsMediaService",
			typeof(bool),
			typeof(PlayerService),
			new PropertyMetadata(null, RegisterAsMediaServicePropertyChanged));
		/// <summary>
		/// Gets value describing whether FrameworkElement is acting as View in MVVM.
		/// </summary>
		public static bool GetRegisterAsMediaService(FrameworkElement target)
		{
			return (bool)target.GetValue(RegisterAsMediaServiceProperty);
		}
		/// <summary>
		/// Sets value describing whether FrameworkElement is acting as View in MVVM.
		/// </summary>
		public static void SetRegisterAsMediaService(FrameworkElement target, bool value)
		{
			target.SetValue(RegisterAsMediaServiceProperty, value);
		}

		private static void RegisterAsMediaServicePropertyChanged(DependencyObject d,
			DependencyPropertyChangedEventArgs e)
		{
			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				MediaElement mediaElement = d as MediaElement;
				if (mediaElement != null)
				{
					bool newValue = (bool)e.NewValue;
					bool oldValue = (bool)e.OldValue;

					if (newValue)
					{
						var playerService = ServiceLocator.Current.GetInstance<IPlayerService>();
						if (playerService != null)
						{
							playerService.RegisterAsMediaService(mediaElement);
						}
					}
				}
			}
		}
		#endregion

		#region FieldsPrivate
		private IDataService m_dataService;
		private IAccountService m_accountService;
		private MediaElement m_mediaElement;
		private Track m_currentTrack;
		private PlayerState m_currentState;
		private bool m_canExecuteNextTrack;
		private bool m_canExecutePreviousTrack;
		private SystemMediaTransportControls m_mediaControls;
		#endregion

		#region Properties
		public Track CurrentTrack
		{
			get
			{
				return this.m_currentTrack;
			}
		}
		public TimeSpan Duration
		{
			get
			{
				return this.m_mediaElement.NaturalDuration.TimeSpan;
			}
		}
		public TimeSpan Position
		{
			get
			{
				return this.m_mediaElement.Position;
			}
		}
		public PlayerState CurrentState
		{
			get
			{
				return this.m_currentState;
			}
			private set
			{
				this.m_currentState = value;
			}
		}
		public bool CanExecuteNextTrack
		{
			get
			{
				return this.m_canExecuteNextTrack;
			}
			set
			{
				this.m_canExecuteNextTrack = value;
			}
		}
		public bool CanExecutePreviousTrack
		{
			get
			{
				return this.m_canExecutePreviousTrack;
			}
			set
			{
				this.m_canExecutePreviousTrack = value;
			}
		}
		#endregion

		#region MethodsPublic
		public PlayerService(IDataService dataService, IAccountService accountService)
		{
			this.CurrentState = Data.Audio.PlayerState.Closed;

			this.m_dataService = dataService;
			this.m_accountService = accountService;

			this.m_mediaControls = SystemMediaTransportControls.GetForCurrentView();
			this.m_mediaControls.ButtonPressed += (sender, args) =>
				{
					switch (args.Button)
					{
						case SystemMediaTransportControlsButton.Play:
							this.Play();
							break;
						case SystemMediaTransportControlsButton.Pause:
							this.Pause();
							break;
						case SystemMediaTransportControlsButton.Stop:
							this.Stop();
							break;
						case SystemMediaTransportControlsButton.Previous:
							this.PreviousTrack();
							break;
						case SystemMediaTransportControlsButton.Next:
							this.NextTrack();
							break;
					}
				};
		}

		void systemControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
		{
			throw new NotImplementedException();
		}
		
		public void RegisterAsMediaService(MediaElement mediaElement)
		{
			if (mediaElement != null && this.m_mediaElement == null)
			{
				this.m_mediaElement = mediaElement;
				//The property AreTransportControlsEnabled causes in an additional displaying of mediaplayer content.
				//this.m_mediaElement.AreTransportControlsEnabled = true;
				this.m_mediaElement.AudioCategory = Windows.UI.Xaml.Media.AudioCategory.BackgroundCapableMedia;
				this.m_mediaElement.MediaOpened += OnMediaOpened;
				this.m_mediaElement.MediaEnded += OnMediaEnded;
				this.m_mediaElement.MediaFailed += OnMediaFailed;
				this.m_mediaElement.CurrentStateChanged += OnCurrentStateChanged;
			}
		}

		public async Task SetTrackAsync(Track track)
		{
			try
			{
				this.m_currentTrack = track;
				if (this.m_currentTrack != null)
				{
					Guid guid = this.m_currentTrack.Guid;
					if (guid != null && !guid.Equals(Guid.Empty))
					{
						//MediaStreamSource mediaStreamSource = new MediaStreamSource()

						//string fileName = 
						//var stream = await this.m_dataService.GetAudioFile(this.m_currentTrack.Guid);
						//if (stream != null)
						//{
						//    var accessStream = await stream.AsRandomAccessStreamAsync();
						//    this.m_mediaElement.SetSource(accessStream, "application/octet-stream");
						//}

						//var accessStream = await this.m_dataService.GetAudioStream(this.m_currentTrack.Guid);
						//this.m_mediaElement.SetSource(accessStream, "audio/mpeg");

						//var tokenResponse = await this.m_accountService.RefreshToken();

						string strUrl = string.Format("{0}/api/files/{1}/", this.m_dataService.ServiceUrl, guid.ToString());
						this.m_mediaElement.Source = new Uri(strUrl);
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async void Play()
		{
			await this.m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				this.m_mediaElement.Play();
			});
		}
		public async void Pause()
		{
			await this.m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				this.m_mediaElement.Pause();
			});
		}
		public async void Stop()
		{
			await this.m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				this.m_mediaElement.Stop();
			});
		}
		public async void NextTrack()
		{
			if (this.CanExecuteNextTrack)
			{
				await this.m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					Messenger.Default.Send(new BSE.Tunes.StoreApp.Messaging.MediaNextPressedMessage());
				});
			}
		}
		public async void PreviousTrack()
		{
			if (this.CanExecutePreviousTrack)
			{
				await this.m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					Messenger.Default.Send(new BSE.Tunes.StoreApp.Messaging.MediaPreviousPressedMessage());
				});
			}
		}
		#endregion

		#region MethodsPrivate
		private void OnCurrentStateChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			this.CurrentState = PlayerState.Closed;
			switch (this.m_mediaElement.CurrentState)
			{
				case Windows.UI.Xaml.Media.MediaElementState.Buffering:
					this.CurrentState = PlayerState.Buffering;
					break;
				case Windows.UI.Xaml.Media.MediaElementState.Opening:
					this.CurrentState = PlayerState.Opening;
					break;
				case Windows.UI.Xaml.Media.MediaElementState.Paused:
					this.CurrentState = PlayerState.Paused;
					this.m_mediaControls.PlaybackStatus = MediaPlaybackStatus.Paused;
					break;
				case Windows.UI.Xaml.Media.MediaElementState.Playing:
					this.CurrentState = PlayerState.Playing;
					this.m_mediaControls.PlaybackStatus = MediaPlaybackStatus.Playing;
					this.m_mediaControls.IsNextEnabled = this.CanExecuteNextTrack;
					this.m_mediaControls.IsPreviousEnabled = this.CanExecutePreviousTrack;
					break;
				case Windows.UI.Xaml.Media.MediaElementState.Stopped:
					this.CurrentState = PlayerState.Stopped;
					this.m_mediaControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
					break;
				default:
					this.CurrentState = PlayerState.Closed;
					this.m_mediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;
					break;
			}
			Messenger.Default.Send<BSE.Tunes.Data.Audio.PlayerState>(this.CurrentState);
		}

		private void OnMediaFailed(object sender, Windows.UI.Xaml.ExceptionRoutedEventArgs e)
		{
			//throw new NotImplementedException();
		}
		private async void OnMediaOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			SystemMediaTransportControlsDisplayUpdater updater = this.m_mediaControls.DisplayUpdater;
			updater.Type = MediaPlaybackType.Music;
			updater.MusicProperties.AlbumArtist = this.CurrentTrack.Album.Artist.Name;
			updater.MusicProperties.Title = this.CurrentTrack.Name;

			//UE 24.03.2014: If the updater.Thumbnail is used, the background task capability was lost
			//
			//if (this.CurrentTrack.Album.Cover != null)
			//{
			//	using (MemoryStream stream = new MemoryStream(this.CurrentTrack.Album.Cover))
			//	{
			//		var t2 = await stream.AsRandomAccessStreamAsync();
			//		updater.Thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromStream(t2);
			//	}
			//}
			
			updater.Update();
			this.m_mediaControls.IsPlayEnabled = true;
			this.m_mediaControls.IsPauseEnabled = true;
			this.m_mediaControls.IsNextEnabled = false;
			this.m_mediaControls.IsPreviousEnabled = false;
			Messenger.Default.Send(new BSE.Tunes.StoreApp.Messaging.MediaOpenedMessage());
		}
		private void OnMediaEnded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			Messenger.Default.Send(new BSE.Tunes.StoreApp.Messaging.MediaEndedMessage());
		}
		#endregion
	}
}
