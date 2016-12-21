using BSE.Tunes.Data;
using BSE.Tunes.Data.Extensions;
using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class RandomPlayerViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IPlayerManager m_playerManager;
        private ObservableCollection<int> m_filteredTrackIds;
        #endregion

        #region Properties
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
        public RandomPlayerViewModel()
        {
            m_playerManager = PlayerManager.Instance;
            LoadData();
        }
        #endregion

        #region MethodsPrivate
        private async void LoadData()
        {
            ObservableCollection<int> trackIds = await DataService.GetTrackIdsByFilters(new Filter());
            if (trackIds != null)
            {
                this.FilteredTrackIds = trackIds.ToRandomCollection();
                int trackId = this.FilteredTrackIds.FirstOrDefault();
                //if (trackId > 0)
                //{
                //    Track track = await this.m_dataService.GetTrackById(trackId);
                //    if (track != null)
                //    {
                //        //Messenger.Default.Send<TrackMessage>(new TrackMessage(track));
                //    }
                //}
                this.m_playerManager.TrackIds = new ObservableCollection<int>(this.FilteredTrackIds);
                //this.PlayRandomTracksCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion
    }
}
