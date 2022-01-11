﻿using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.IO;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using MediaParsers;
using System;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
                MediaPlayerElement mediaElement = d as MediaPlayerElement;
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
        private IAuthenticationService m_accountService;
        private MediaPlayerElement m_mediaElement;
        private MediaPlayer m_mediaPlayer;
        private Track m_currentTrack;
        private PlayerState m_currentState;
        private bool m_canExecuteNextTrack;
        private bool m_canExecutePreviousTrack;
        private TimeSpan m_playerNaturalDuration = new TimeSpan(0);
        private TimeSpan m_playerPosition;
        private SystemMediaTransportControls m_systemMediaControls;

        private AudioStreamDownloader m_audioStreamDownloader;
        private AudioStreamDownloader m_audioStreamPreloader;
        private MediaStreamSource m_mediaStreamSource;
        private UInt64 m_byteOffset;
        private TimeSpan m_timeOffset;
        private IRandomAccessStream m_mediaStream;

        // MP3 Framesize and length for Layer II and Layer III
        private const UInt32 sampleSize = 1152;
        private TimeSpan sampleDuration = new TimeSpan(0, 0, 0, 0, 70);

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
                return this.m_playerNaturalDuration;
            }
            private set
            {
                this.m_playerNaturalDuration = value;
            }
        }
        public TimeSpan Position
        {
            get
            {
                return this.m_playerPosition;
            }
            private set
            {
                this.m_playerPosition = value;
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
        public MediaPlayer MediaPlayer => m_mediaPlayer ?? (m_mediaPlayer = new MediaPlayer());
        #endregion

        #region MethodsPublic
        public PlayerService(IDataService dataService, IAuthenticationService accountService)
        {
            this.CurrentState = PlayerState.Closed;

            this.m_dataService = dataService;
            this.m_accountService = accountService;
        }

        public void RegisterAsMediaService(MediaPlayerElement mediaElement)
        {
            if (mediaElement != null && this.m_mediaElement == null)
            {
                this.m_mediaElement = mediaElement;
                this.m_mediaElement.SetMediaPlayer(MediaPlayer);
                this.m_mediaElement.AutoPlay = true;
                this.m_mediaElement.MediaPlayer.MediaOpened += OnMediaOpened;
                this.m_mediaElement.MediaPlayer.MediaEnded += OnMediaEnded;
                this.m_mediaElement.MediaPlayer.MediaFailed += OnMediaFailed;
                this.m_mediaElement.MediaPlayer.PlaybackSession.PlaybackStateChanged += OnPlaybackStateChanged;

                this.m_systemMediaControls = this.m_mediaElement.MediaPlayer.SystemMediaTransportControls;
                this.m_systemMediaControls.IsEnabled = false;
                this.m_systemMediaControls.ButtonPressed += (sender, args) =>
                {
                    switch (args.Button)
                    {
                        case SystemMediaTransportControlsButton.Play:
                            this.Play();
                            break;
                        case SystemMediaTransportControlsButton.Pause:
                            this.Pause();
                            break;
                        case SystemMediaTransportControlsButton.Previous:
                            this.PreviousTrack();
                            break;
                        case SystemMediaTransportControlsButton.Next:
                            this.NextTrack();
                            break;
                    }
                };
                this.m_systemMediaControls.IsPlayEnabled = true;
                this.m_systemMediaControls.IsPauseEnabled = true;
                this.m_systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;
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
                        string strUrl = string.Format("{0}/api/files/audio/{1}", m_dataService.ServiceUrl, guid.ToString());

                        this.m_audioStreamDownloader = new AudioStreamDownloader(this.m_dataService);
                        this.m_audioStreamDownloader.DownloadProgessStarted += OnDownloadProgessStarted;
                        m_audioStreamDownloader.DownloadComplete += OnDownloadComplete;
                        m_audioStreamPreloader?.Cancel();
                        await m_audioStreamDownloader.DownloadAsync(new Uri(strUrl), guid);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task PrepareTrack(Track track)
        {
            try
            {
                Guid guid = track.Guid;
                if (guid != null && !guid.Equals(Guid.Empty))
                {
                    string strUrl = string.Format("{0}/api/files/audio/{1}", m_dataService.ServiceUrl, guid.ToString());
                    m_audioStreamPreloader = new AudioStreamDownloader(this.m_dataService);
                    m_audioStreamPreloader.PreloadComplete += OnPreloadComplete;
                    await m_audioStreamPreloader.PreloadAsync(new Uri(strUrl), guid);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async void Play()
        {
            await this.m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.m_mediaElement.MediaPlayer.Play();
            });
        }
        public async void Pause()
        {
            await this.m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.m_mediaElement.MediaPlayer.Pause();
            });
        }
        public async void NextTrack()
        {
            if (this.CanExecuteNextTrack)
            {
                await this.m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Messenger.Default.Send(new MediaStateChangedArgs(MediaState.NextRequested));
                });
            }
        }
        public async void PreviousTrack()
        {
            if (this.CanExecutePreviousTrack)
            {
                await this.m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Messenger.Default.Send(new MediaStateChangedArgs(MediaState.PreviousRequested));
                });
            }
        }
        #endregion

        #region MethodsPrivate
        private void OnDownloadProgessStarted(object sender, EventArgs e)
        {
            // initialize Parsing Variables
            this.m_byteOffset = 0;
            this.m_timeOffset = new TimeSpan(0);

            if (this.m_audioStreamDownloader.TotalBytesToReceive > 0)
            {
                var stream = this.m_audioStreamDownloader.Stream;
                MpegFrame mpegFrame = stream.ReadPastId3V2Tags();

                AudioEncodingProperties audioProps = AudioEncodingProperties.CreateMp3((uint)mpegFrame.SamplingRate, 2, (uint)mpegFrame.Bitrate);
                AudioStreamDescriptor audioDescriptor = new AudioStreamDescriptor(audioProps);

                //close the MediaStreamSource and remove the MediaStreamSource event handlers
                CloseMediaStreamSource(this.m_mediaStreamSource);

                this.m_mediaStreamSource = new Windows.Media.Core.MediaStreamSource(audioDescriptor);
                this.m_mediaStreamSource.CanSeek = true;
                this.m_mediaStreamSource.Duration = this.m_currentTrack.Duration;

                // hooking up the MediaStreamSource event handlers 
                this.m_mediaStreamSource.Starting += OnStreamSourceStarting;
                this.m_mediaStreamSource.SampleRequested += OnStreamSourceSampleRequested;
                this.m_mediaStreamSource.Closed += OnStreamSourceClosed;

                //this.m_mediaElement.SetMediaStreamSource(this.m_mediaStreamSource);
                this.m_mediaElement.MediaPlayer.Source = MediaSource.CreateFromMediaStreamSource(this.m_mediaStreamSource);
            }
        }
        private void OnDownloadComplete(object sender, EventArgs e)
        {
            AudioStreamDownloader downloader = sender as AudioStreamDownloader;
            if (downloader != null)
            {
                downloader.DownloadComplete -= OnDownloadComplete;
            }
            Messenger.Default.Send(new MediaStateChangedArgs(MediaState.DownloadCompleted));
        }
        private void OnPreloadComplete(object sender, EventArgs e)
        {
            AudioStreamDownloader downloader = sender as AudioStreamDownloader;
            if (downloader != null)
            {
                downloader.PreloadComplete -= OnPreloadComplete;
                downloader.Dispose();
            }
        }
        /// <summary>
        /// Occurs when the MediaStreamSource is ready to start requesting MediaStreamSample objects.
        /// </summary>
        /// <param name="sender">Represents a media source that delivers media samples directly to the media pipeline.</param>
        /// <param name="args">Provides data for the MediaStreamSource.Starting event.</param>
        private void OnStreamSourceStarting(MediaStreamSource sender, MediaStreamSourceStartingEventArgs args)
        {
            MediaStreamSourceStartingRequest request = args.Request;
            if ((request.StartPosition != null) && request.StartPosition.Value <= m_mediaStreamSource.Duration)
            {
                UInt64 sampleOffset = (UInt64)request.StartPosition.Value.Ticks / (UInt64)sampleDuration.Ticks;
                this.m_timeOffset = new TimeSpan((long)sampleOffset * sampleDuration.Ticks);
                this.m_byteOffset = sampleOffset * sampleSize;
            }

            if (this.m_mediaStream == null)
            {
                MediaStreamSourceStartingRequestDeferral deferal = request.GetDeferral();
                try
                {
                    this.m_mediaStream = this.m_audioStreamDownloader.Stream;
                    request.SetActualStartPosition(new TimeSpan());
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    deferal.Complete();
                }
            }
            else
            {
                request.SetActualStartPosition(m_timeOffset);
            }
        }
        /// <summary>
        /// Occurs when the MediaStreamSource request a MediaStreamSample for a specified stream.
        /// </summary>
        /// <param name="sender">Represents a media source that delivers media samples directly to the media pipeline.</param>
        /// <param name="args">Provides the data for the SampleRequested event.</param>
        private async void OnStreamSourceSampleRequested(MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            MediaStreamSourceSampleRequest request = args.Request;
            // check if the sample requested byte offset is within the file size
            if (this.m_byteOffset + sampleSize <= (ulong)this.m_audioStreamDownloader.TotalBytesToReceive)
            {
                //Calculate the current position within the track
                double ratio = (double)this.m_byteOffset / (double)this.m_audioStreamDownloader.TotalBytesToReceive;
                this.m_playerPosition = new TimeSpan((long)(this.CurrentTrack.Duration.Ticks * ratio));

                MediaStreamSourceSampleRequestDeferral deferal = request.GetDeferral();

                var inputStream = m_mediaStream.GetInputStreamAt(this.m_byteOffset);
                if (inputStream != null)
                {
                    // create the MediaStreamSample and assign to the request object. 
                    // You could also create the MediaStreamSample using createFromBuffer(...)
                    MediaStreamSample sample = await MediaStreamSample.CreateFromStreamAsync(inputStream, sampleSize, m_timeOffset);
                    sample.Duration = sampleDuration;
                    sample.KeyFrame = true;

                    // increment the time and byte offset
                    this.m_byteOffset += sampleSize;
                    this.m_timeOffset = this.m_timeOffset.Add(sampleDuration);
                    request.Sample = sample;
                }
                deferal.Complete();
            }
        }
        /// <summary>
        /// Occurs when the MediaStreamSource is shutting down.
        /// </summary>
        /// <param name="sender">Represents a media source that delivers media samples directly to the media pipeline.</param>
        /// <param name="args">Provides data for the MediaStreamSource.Closed event.</param>
        private void OnStreamSourceClosed(MediaStreamSource sender, MediaStreamSourceClosedEventArgs args)
        {
            if (sender == m_mediaStreamSource)
            {
                CloseMediaStreamSource(sender);
            }
        }
        /// <summary>
        /// Close the MediaStreamSource and remove the MediaStreamSource event handlers
        /// </summary>
        /// <param name="mediaStreamSource"></param>
        private void CloseMediaStreamSource(MediaStreamSource mediaStreamSource)
        {
            // close the MediaStreamSource and remove the MediaStreamSource event handlers
            if (this.m_mediaStream != null)
            {
                this.m_mediaStream.Dispose();
                this.m_mediaStream = null;
            }
            if (mediaStreamSource != null)
            {
                mediaStreamSource.SampleRequested -= OnStreamSourceSampleRequested;
                mediaStreamSource.Starting -= OnStreamSourceStarting;
                mediaStreamSource.Closed -= OnStreamSourceClosed;
                m_mediaStreamSource = null;
            }
        }

        private async void OnPlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            await m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.CurrentState = PlayerState.Closed;
                MediaPlayer mediaPlayer = sender.MediaPlayer;
                if (mediaPlayer != null)
                {
                    switch (mediaPlayer.PlaybackSession.PlaybackState)
                    {
                        case MediaPlaybackState.Buffering:
                            this.CurrentState = PlayerState.Buffering;
                            break;
                        case MediaPlaybackState.Opening:
                            this.CurrentState = PlayerState.Opening;
                            break;
                        case MediaPlaybackState.Paused:
                            this.CurrentState = PlayerState.Paused;
                            this.m_systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                            break;
                        case MediaPlaybackState.Playing:
                            this.CurrentState = PlayerState.Playing;
                            this.m_systemMediaControls.IsNextEnabled = this.CanExecuteNextTrack;
                            this.m_systemMediaControls.IsPreviousEnabled = this.CanExecutePreviousTrack;
                            break;
                        default:
                            this.CurrentState = PlayerState.Closed;
                            this.m_systemMediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                            break;
                    }
                    Messenger.Default.Send(new Mvvm.Messaging.PlayerStateChangedArgs(this.CurrentState));
                }
            });
        }
        private void OnMediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            //throw new NotImplementedException();
        }
        private async void OnMediaOpened(MediaPlayer sender, object args)
        {
            await m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.m_playerNaturalDuration = new TimeSpan(this.m_mediaElement.MediaPlayer.PlaybackSession.NaturalDuration.Ticks);
                this.m_playerPosition = new TimeSpan();
                this.m_systemMediaControls.IsEnabled = true;

                SystemMediaTransportControlsDisplayUpdater updater = this.m_systemMediaControls.DisplayUpdater;
                updater.Type = MediaPlaybackType.Music;
                updater.MusicProperties.Artist = this.CurrentTrack.Album.Artist.Name;
                updater.MusicProperties.AlbumArtist = this.CurrentTrack.Album.Artist.Name;
                updater.MusicProperties.AlbumTitle = this.CurrentTrack.Album.Title;
                updater.MusicProperties.Title = this.CurrentTrack.Name;
                updater.Thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(this.m_dataService.GetImage(this.CurrentTrack.Album.AlbumId, true));
                updater.Update();

                this.m_systemMediaControls.IsPlayEnabled = true;
                this.m_systemMediaControls.IsPauseEnabled = true;
                this.m_systemMediaControls.IsNextEnabled = false;
                this.m_systemMediaControls.IsPreviousEnabled = false;

                Messenger.Default.Send(new MediaStateChangedArgs(MediaState.Opened));
            });
        }
        private async void OnMediaEnded(MediaPlayer sender, object args)
        {
            await m_mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Messenger.Default.Send(new MediaStateChangedArgs(MediaState.Ended));
            });
        }
        #endregion
    }
}