using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSE.Tunes.StoreApp.Models
{
    public enum PlayerState
    {
        //The Player contains no media.
        Closed = 0,
        Opening = 1,
        Buffering = 2,
        Playing = 3,
        Paused = 4,
        Stopped = 5
    }
}
