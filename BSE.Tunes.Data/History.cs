using BSE.Tunes.Data.Audio;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BSE.Tunes.Data
{
    [DataContract]
    public class History
    {
        #region
        /// <summary>
        /// Gets or sets the <see cref="PlayerMode"/>.
        /// </summary>
        [DataMember]
        public PlayerMode PlayMode
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the AppId
        /// </summary>
        public int AppId
        {
            get
            {
                return (this.PlayMode == PlayerMode.Playlist) ? (int)PlayerMode.Song : (int)this.PlayMode;
            }
        }
        /// <summary>
        /// Gets or sets the time when the song was played
        /// </summary>
        [DataMember]
        public DateTime PlayedAt
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Id of the played album.
        /// </summary>
        [DataMember]
        public int AlbumId
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Id of the played track
        /// </summary>
        [DataMember]
        public int TrackId
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the UserName
        /// </summary>
        [DataMember]
        public string UserName
        {
            get;
            set;
        }
        #endregion
    }
}
