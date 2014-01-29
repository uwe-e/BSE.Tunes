using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Event
{
    public class PlaylistChangedEventArgs : EventArgs
    {
        #region Properties
        public Playlist Playlist
        {
            get;
            private set;
        }
        #endregion

        #region MethodsPublic
        public PlaylistChangedEventArgs(Playlist playlist)
        {
            this.Playlist = playlist;
        }
        #endregion
    }
}
