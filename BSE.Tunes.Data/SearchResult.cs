using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BSE.Tunes.Data
{
    [DataContract]
    public class SearchResult
    {
        #region Properties
        /// <summary>
        /// Gets or sets an array of <see cref="Album"/> objects.
        /// </summary>
        [DataMember]
        public Album[] Albums { get; set; }
        /// <summary>
        /// Gets or sets an array of <see cref="Track"/> objects.
        /// </summary>
        [DataMember]
        public Track[] Tracks { get; set; }
        #endregion
    }
}
