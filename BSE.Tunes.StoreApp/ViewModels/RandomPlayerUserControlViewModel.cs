using BSE.Tunes.Data;
using BSE.Tunes.Data.Extensions;
using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Messaging;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class RandomPlayerUserControlViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private readonly IDataService m_dataService;
        private readonly PlayerManager m_playerManager;
        private RelayCommand m_playRandomTracksCommand;
        private ObservableCollection<Track> m_filteredTracks;
        #endregion

        #region Properties
        public RelayCommand PlayRandomTracksCommand
        {
            get
            {
                return this.m_playRandomTracksCommand ??
                    (this.m_playRandomTracksCommand = new RelayCommand(this.PlayRandomTracks, this.CanExecutePlayRandomTracks));
            }
        }
        public ObservableCollection<Track> FilteredTracks
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
        #endregion

        #region MethodsPublic
        public RandomPlayerUserControlViewModel(IDataService dataService, PlayerManager playerManager)
        {
            this.m_dataService = dataService;
            this.m_playerManager = playerManager;
            this.LoadData();
        }
        #endregion

        #region MethodsPrivate
        private async void LoadData()
        {
            ObservableCollection<Track> tracks = await this.m_dataService.GetTracksByFilters(new Filter());
            if (tracks != null)
            {
                this.FilteredTracks = tracks.ToRandomCollection();
                Track track = this.FilteredTracks.FirstOrDefault();
                if (track != null)
                {
                    track = await this.m_dataService.GetTrackById(track.Id);
                    if (track != null)
                    {
                        Messenger.Default.Send<TrackMessage>(new TrackMessage(track));
                    }
                }
                this.m_playerManager.Tracks = new ObservableCollection<Track>(this.FilteredTracks);
                this.PlayRandomTracksCommand.RaiseCanExecuteChanged();
            }
        }
        private bool CanExecutePlayRandomTracks()
        {
            return this.FilteredTracks != null && this.FilteredTracks.Count > 0;
        }
        private void PlayRandomTracks()
        {
            if (this.FilteredTracks != null)
            {
                this.FilteredTracks = this.FilteredTracks.ToRandomCollection();
                this.m_playerManager.PlayTracks(this.FilteredTracks, Data.Audio.PlayerMode.Random);
            }
        }
        #endregion
    }
}
