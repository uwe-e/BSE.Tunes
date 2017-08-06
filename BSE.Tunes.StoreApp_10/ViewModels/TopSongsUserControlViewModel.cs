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
    public class TopSongsUserControlViewModel : FeaturedItemsBaseViewModel
    {
        #region FieldsPrivate
        private ICommand m_showAlbumCommand;
        #endregion

        #region Properties
        public ICommand ShowAlbumCommand => m_showAlbumCommand ?? (m_showAlbumCommand = new RelayCommand<GridPanelItemViewModel>(ShowAlbum));
        #endregion

        #region MethodsPublic
        public override async void LoadData()
        {
            var tracks = await DataService.GetTopTracks(0,10);
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
