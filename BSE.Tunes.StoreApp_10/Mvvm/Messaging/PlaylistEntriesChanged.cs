using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class PlaylistEntriesChangedArgs : PlaylistChangedArgs
    {
        public PlaylistEntriesChangedArgs(Playlist playlist) : base(playlist)
        {
        }
    }
}
