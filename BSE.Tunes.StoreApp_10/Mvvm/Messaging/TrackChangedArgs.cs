using BSE.Tunes.Data;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class TrackChangedArgs : MessageBase
    {
        public Track Track{get;private set;}
        public TrackChangedArgs(Track track)
        {
            this.Track = track;
        }
    }
}
