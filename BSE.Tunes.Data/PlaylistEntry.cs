using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BSE.Tunes.Data
{
    public class PlaylistEntry
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the id of the playlist.
        /// </summary>
        [DataMember]
        public int PlaylistId { get; set; }
        /// <summary>
        /// Gets or sets the id of the track.
        /// </summary>
        [DataMember]
        public int TrackId { get; set; }
        /// <summary>
        /// Gets or sets the name of the track.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the name of the artist.
        /// </summary>
        [DataMember]
        public string Artist { get; set; }
        /// <summary>
        /// Gets or sets the duration oft the track.
        /// </summary>
        [DataMember]
        public TimeSpan Duration { get; set; }
        /// <summary>
        /// Gets or sets the guid.
        /// </summary>
        [DataMember]
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }
    }
}
