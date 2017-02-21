using BSE.Tunes.Data;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistFlyoutItemViewModel : MenuFlyoutItemViewModel
    {
        #region FieldsPrivate
        private Playlist m_playlist;
        private ICommand m_addSelectedToPlaylistCommand;
        #endregion

        #region Properties
        public Playlist Playlist
        {
            get
            {
                return m_playlist;
            }
            set
            {
                m_playlist = value;
                RaisePropertyChanged("Playlist");
            }
        }
        public ICommand AddSelectedToPlaylistCommand => m_addSelectedToPlaylistCommand ?? (m_addSelectedToPlaylistCommand = new RelayCommand<object>(AddSelectedToPlaylist));

        private void AddSelectedToPlaylist(object obj)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
