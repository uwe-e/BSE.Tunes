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
        private ObservableCollection<int> m_filteredTrackIds;
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
        public ObservableCollection<int> FilteredTrackIds
        {
            get
            {
                return this.m_filteredTrackIds;
            }
            set
            {
                this.m_filteredTrackIds = value;
                RaisePropertyChanged("FilteredTrackIds");
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
        public override void ResetData()
        {
            base.ResetData();
            this.LoadData();
        }
        #endregion

        #region MethodsPrivate
        private async void LoadData()
        {
            ObservableCollection<int> trackIds = await this.m_dataService.GetTrackIdsByFilters(new Filter());
            if (trackIds != null)
            {
                this.FilteredTrackIds = trackIds.ToRandomCollection();
                int trackId = this.FilteredTrackIds.FirstOrDefault();
                if (trackId > 0)
                {
                    Track track = await this.m_dataService.GetTrackById(trackId);
                    if (track != null)
                    {
                        Messenger.Default.Send<TrackMessage>(new TrackMessage(track));
                    }
                }
                this.m_playerManager.TrackIds = new ObservableCollection<int>(this.FilteredTrackIds);
                this.PlayRandomTracksCommand.RaiseCanExecuteChanged();
            }
        }
        private bool CanExecutePlayRandomTracks()
        {
            return this.FilteredTrackIds != null && this.FilteredTrackIds.Count > 0;
        }
        private void PlayRandomTracks()
        {
            if (this.FilteredTrackIds != null)
            {
                this.FilteredTrackIds = this.FilteredTrackIds.ToRandomCollection();
                this.m_playerManager.PlayTracks(this.FilteredTrackIds, Data.Audio.PlayerMode.Random);
            }
        }
        #endregion
    }
}