using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BSE.Tunes.Data
{
    [DataContract]
    public class SystemInfo
    {
        [DataMember]
        public int NumberTracks
        {
            get;
            set;
        }
    }
}
