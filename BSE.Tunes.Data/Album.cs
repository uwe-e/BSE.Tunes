using System;
using System.Runtime.Serialization;

namespace BSE.Tunes.Data
{
    [DataContract]
    public class Album
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public Artist Artist { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public int? Year { get; set; }
        [DataMember]
        public byte[] Thumbnail { get; set; }
        [DataMember]
        public byte[] Cover { get; set; }
        [DataMember]
        public Genre Genre { get; set; }
        [DataMember]
        public Track[] Tracks { get; set; }
		[DataMember]
		public Guid AlbumId { get; 	set;}
    }
}
