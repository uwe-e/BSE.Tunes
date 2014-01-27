using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BSE.Tunes.Data
{
    [DataContract]
    public class Track
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int TrackNumber { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public TimeSpan Duration { get; set; }
        [DataMember]
        public Guid Guid { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public Album Album { get; set; }
    }
}
