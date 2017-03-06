using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class PlaylistDeletedArgs : PlaylistChangedArgs
    {
        public PlaylistDeletedArgs(Playlist playlist) : base(playlist)
        {
        }
    }
}
