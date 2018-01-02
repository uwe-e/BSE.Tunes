using BSE.Tunes.Data;
using BSE.Tunes.Data.Extensions;
using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class RandomPlayerPanelUserControlViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IPlayerManager m_playerManager;
        private ObservableCollection<int> m_filteredTrackIds;
        private string m_text;
        private RelayCommand m_playRandomTracksCommand;
        #endregion

        #region Properties
        public ObservableCollection<int> FilteredTrackIds
        {
            get
            {
                return m_filteredTrackIds;
            }
            set
            {
                m_filteredTrackIds = value;
                RaisePropertyChanged("FilteredTrackIds");
            }
        }
        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
                RaisePropertyChanged("Text");
            }
        }
        public RelayCommand PlayRandomTracksCommand => m_playRandomTracksCommand ?? (m_playRandomTracksCommand = new RelayCommand(PlayRandomTracks, CanExecutePlayRandomTracks));
        #endregion

        #region MethodsPublic
        public RandomPlayerPanelUserControlViewModel()
        {
            m_playerManager = PlayerManager.Instance;
            LoadData();
        }
        #endregion

        #region MethodsPrivate
        private async void LoadData()
        {
            //Get the id's of all playable tracks and randomize it
            ObservableCollection<int> trackIds = await DataService.GetTrackIdsByGenre();
            if (trackIds != null)
            {
                FilteredTrackIds = trackIds.ToRandomCollection();
                int trackId = FilteredTrackIds.FirstOrDefault();
                if (trackId > 0)
                {
                    Track track = await DataService.GetTrackById(trackId);
                    if (track != null)
                    {
                        Messenger.Default.Send(new TrackChangedArgs(track));
                    }
                }
                m_playerManager.Playlist = FilteredTrackIds.ToNavigableCollection();
                PlayRandomTracksCommand.RaiseCanExecuteChanged();
            }
            //Gets the number of tracks and builds the panel text.
            SystemInfo sysInfo = await DataService.GetSystemInfo();
            Text = string.Format(CultureInfo.CurrentCulture, ResourceService.GetString("RandomPlayerPanel_Text"), sysInfo?.NumberTracks ?? 0);
        }
        private bool CanExecutePlayRandomTracks()
        {
            return FilteredTrackIds?.Count > 0;
        }

        private void PlayRandomTracks()
        {
            FilteredTrackIds = FilteredTrackIds?.ToRandomCollection();
            m_playerManager.PlayTracks(this.FilteredTrackIds, PlayerMode.Random);
        }
        #endregion
    }
}
