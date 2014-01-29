using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BSE.Tunes.Data
{
    public class Playlist
    {
        #region FieldsPrivate
        private IList<PlaylistEntry> m_playlistEntries;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int Id
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the guid.
        /// </summary>
        [DataMember]
        public System.Guid Guid
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        [DataMember]
        public string UserName
        {
            get;
            set;
        }
        [DataMember]
        public int NumberEntries
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets an arry of <see cref="PlaylistEntry"/> entries.
        /// </summary>
        [DataMember]
        public IList<PlaylistEntry> Entries
        {
            get
            {
                return this.m_playlistEntries ??
                    (this.m_playlistEntries = new List<PlaylistEntry>());
            }
        }
        #endregion

        #region MethodsPublic
        /// <summary>
        /// Determines whether the specified <see cref="Playlist"/> is equal to the current <see cref="Playlist"/>. 
        /// </summary>
        /// <param name="obj">The Object to compare with the current <see cref="Playlist"/>.</param>
        /// <returns>true if the specified <see cref="Playlist"/> is equal to the current <see cref="Playlist"/>; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            Playlist playlist = obj as Playlist;
            if (playlist == null)
            {
                return false;
            }
            if (this.Id.Equals(playlist.Id) == false)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Playlist"/></returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        #endregion
    }
}
