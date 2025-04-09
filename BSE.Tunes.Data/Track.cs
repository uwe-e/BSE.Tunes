using System;
using System.Runtime.Serialization;

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
        public string Extension { get; set; }
        [DataMember]
        public Album Album { get; set; }
    }
}
