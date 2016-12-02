using BSE.Tunes.Data;
using BSE.Tunes.Data.Audio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Managers
{
    public interface IPlayerManager
    {
        ObservableCollection<int> TrackIds
        {
            get;set;
        }
        PlayerMode PlayerMode { get; }
        Track CurrentTrack { get; }
        TimeSpan Duration { get; }
        TimeSpan Position { get; }
        bool CanExecuteNextTrack();
        bool CanExecutePreviousTrack();
        bool CanExecutePlay();
        void ExecuteNextTrack();
        void ExecutePreviousTrack();
        Task SetTrackAsync(Track track);
        void Play();
        void Pause();
        void Stop();
    }
}
