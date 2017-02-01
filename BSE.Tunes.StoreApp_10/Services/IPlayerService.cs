using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BSE.Tunes.StoreApp.Services
{
    public interface IPlayerService
    {
        TimeSpan Duration { get; }
        TimeSpan Position { get; }
        Track CurrentTrack { get; }
        PlayerState CurrentState { get; }
        bool CanExecuteNextTrack { get; set; }
        bool CanExecutePreviousTrack { get; set; }
        void RegisterAsMediaService(MediaPlayerElement mediaElement);
        Task SetTrackAsync(Track track);
        Task PrepareTrack(Track track);
        void Play();
        void Pause();
        void Stop();
        void NextTrack();
        void PreviousTrack();
    }
}
