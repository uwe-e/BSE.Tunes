using BSE.Tunes.Data;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class PlaylistChangedArgs :MessageBase
    {
        public Playlist Playlist
        {
            get; private set;
        }
        public PlaylistChangedArgs(Playlist playlist)
        {
            Playlist = playlist;
        }
    }
}
