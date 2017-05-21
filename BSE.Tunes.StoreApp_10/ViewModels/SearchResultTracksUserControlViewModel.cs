using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultTracksUserControlViewModel : FeaturedItemsBaseViewModel
    {
        #region FieldsPrivate
        private Query m_query;
        private ICommand m_showAlbumCommand;
        #endregion

        #region Properties
        public Query Query
        {
            get
            {
                return m_query;
            }
            set
            {
                m_query = value;
                RaisePropertyChanged(() => Query);
            }
        }
        public ICommand ShowAlbumCommand => m_showAlbumCommand ?? (m_showAlbumCommand = new RelayCommand<GridPanelItemViewModel>(ShowAlbum));
        #endregion

        #region MethodsPublic
        public SearchResultTracksUserControlViewModel()
        {
        }
        public SearchResultTracksUserControlViewModel(Query query) : base()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Query = query;
                LoadData();
            }
        }
        public async override void LoadData()
        {
            if (Query != null)
            {
                var tracks = await DataService.GetTrackSearchResults(Query);
                if (tracks != null)
                {
                    foreach (var track in tracks)
                    {
                        if (track != null)
                        {
                            Items.Add(new GridPanelItemViewModel
                            {
                                Data = track
                            });
                        }
                    }
                }
            }
        }
        public override void SelectItem(GridPanelItemViewModel item)
        {
            PlayerManager.PlayTrack(((Track)item.Data).Id, PlayerMode.Song);
        }
        #endregion

        #region MethodsPrivate
        private void ShowAlbum(GridPanelItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), (Album)((Track)item.Data).Album);
        }
        #endregion
    }
}
