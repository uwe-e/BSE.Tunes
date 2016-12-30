using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Models
{
    public enum MediaState
    {
        None = 0,
        Opened = 1,
        Ended = 2,
        NextRequested = 3,
        PreviousRequested = 4,
        DownloadCompleted = 5
    }
}
