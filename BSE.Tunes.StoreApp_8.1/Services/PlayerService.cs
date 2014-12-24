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
using BSE.Tunes.StoreApp.IO;
using Windows.Storage.Streams;
using Windows.Media.MediaProperties;
using MediaParsers;

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
        private TimeSpan m_playerNaturalDuration = new TimeSpan(0);
        private TimeSpan m_playerPosition;
        private SystemMediaTransportControls m_mediaControls;

        private AudioStreamDownloader m_audioStreamDownloader;
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
        #endregion

        #region MethodsPublic
        public PlayerService(IDataService dataService, IAccountService accountService)
        {
            this.CurrentState = Data.Audio.PlayerState.Closed;

            this.m_dataService = dataService;
            this.m_accountService = accountService;

            this.m_mediaControls = SystemMediaTransportControls.GetForCurrentView();
            this.m_mediaControls.IsEnabled = false;
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
            this.m_mediaControls.IsPlayEnabled = true;
            this.m_mediaControls.IsPauseEnabled = true;
            this.m_mediaControls.IsStopEnabled = true;
            this.m_mediaControls.PlaybackStatus = MediaPlaybackStatus.Closed;
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
                        string strUrl = string.Format("{0}/api/files/{1}", this.m_dataService.ServiceUrl, guid.ToString());

                        this.m_audioStreamDownloader = new AudioStreamDownloader(this.m_dataService);
                        this.m_audioStreamDownloader.DownloadProgessStarted += OnDownloadProgessStarted;
                        await m_audioStreamDownloader.DownloadAsync(new Uri(strUrl), guid);
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
        private void Reset()
        {
            if (this.m_audioStreamDownloader != null)
            {
                this.m_audioStreamDownloader.DownloadProgessStarted -= OnDownloadProgessStarted;
                this.m_audioStreamDownloader.Close();
                this.m_audioStreamDownloader.Dispose();
                this.m_audioStreamDownloader = null;
            }
            if (this.m_mediaStream != null)
            {
                this.m_mediaStream.Dispose();
                this.m_mediaStream = null;
            }
            if (this.m_mediaStreamSource != null)
            {
                try
                {
                    this.m_mediaStreamSource.SampleRequested -= OnStreamSourceSampleRequested;
                    this.m_mediaStreamSource.Starting -= OnStreamSourceStarting;
                    this.m_mediaStreamSource.Closed -= OnStreamSourceClosed;
                }
                catch (Exception ex)
                {
                }
                this.m_mediaStreamSource = null;
            }
        }
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

                this.m_mediaStreamSource = new Windows.Media.Core.MediaStreamSource(audioDescriptor);
                this.m_mediaStreamSource.CanSeek = true;
                this.m_mediaStreamSource.Duration = this.m_currentTrack.Duration;

                // hooking up the MediaStreamSource event handlers 
                this.m_mediaStreamSource.Starting += OnStreamSourceStarting;
                this.m_mediaStreamSource.SampleRequested += OnStreamSourceSampleRequested;
                this.m_mediaStreamSource.Closed += OnStreamSourceClosed;

                this.m_mediaElement.SetMediaStreamSource(this.m_mediaStreamSource);
            }
        }
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
                    deferal.Complete();
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                request.SetActualStartPosition(m_timeOffset);
            }
        }

        private async void OnStreamSourceSampleRequested(MediaStreamSource sender, MediaStreamSourceSampleRequestedEventArgs args)
        {
            try
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

                    // create the MediaStreamSample and assign to the request object. 
                    // You could also create the MediaStreamSample using createFromBuffer(...)
                    MediaStreamSample sample = await MediaStreamSample.CreateFromStreamAsync(inputStream, sampleSize, m_timeOffset);
                    sample.Duration = sampleDuration;
                    sample.KeyFrame = true;

                    // increment the time and byte offset
                    this.m_byteOffset += sampleSize;
                    this.m_timeOffset = this.m_timeOffset.Add(sampleDuration);
                    request.Sample = sample;
                    deferal.Complete();
                }
                else
                {

                }
            }
            catch (Exception exc)
            {
            }
        }

        private void OnStreamSourceClosed(MediaStreamSource sender, MediaStreamSourceClosedEventArgs args)
        {
            // close the MediaStreamSource and remove the MediaStreamSource event handlers
            if (this.m_mediaStream != null)
            {
                this.m_mediaStream.Dispose();
                this.m_mediaStream = null;
            }

            sender.SampleRequested -= OnStreamSourceSampleRequested;
            sender.Starting -= OnStreamSourceStarting;
            sender.Closed -= OnStreamSourceClosed;

            if (sender == m_mediaStreamSource)
            {
                m_mediaStreamSource = null;
            }
        }
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
        private void OnMediaOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.m_playerNaturalDuration = this.m_mediaElement.NaturalDuration.TimeSpan;
            this.m_playerPosition = new TimeSpan();
            this.m_mediaControls.IsEnabled = true;

            SystemMediaTransportControlsDisplayUpdater updater = this.m_mediaControls.DisplayUpdater;
            updater.Type = MediaPlaybackType.Music;
            updater.MusicProperties.AlbumArtist = this.CurrentTrack.Album.Artist.Name;
            updater.MusicProperties.Title = this.CurrentTrack.Name;
            updater.Thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(this.m_dataService.GetImage(this.CurrentTrack.Album.AlbumId, true));
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