using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistMenuItemViewModel : MenuItemViewModel
    {
        private Playlist m_playlist;
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
        #endregion
    }
}
