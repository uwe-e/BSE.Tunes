using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class PlaylistCreatedArgs : PlaylistChangedArgs
    {
        public InsertMode InsertMode
        {
            get; private set;
        }
        public PlaylistCreatedArgs(Playlist playlist, InsertMode insertMode) : base(playlist) {
            InsertMode = insertMode;
        }
    }
}
