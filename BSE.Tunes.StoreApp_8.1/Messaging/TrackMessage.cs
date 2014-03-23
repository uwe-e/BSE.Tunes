using BSE.Tunes.Data;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Messaging
{
    public class TrackMessage : MessageBase
    {
        public Track Track{get;private set;}
        public TrackMessage(Track track)
        {
            this.Track = track;
        }
    }
}
