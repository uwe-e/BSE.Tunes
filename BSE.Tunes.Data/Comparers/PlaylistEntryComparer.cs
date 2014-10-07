using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSE.Tunes.Data.Comparers
{
    /// <summary>
    /// Defines methods to support the comparison of <see cref="PlaylistEntry"/> objects for equality.
    /// </summary>
    public class PlaylistEntryComparer : IEqualityComparer<PlaylistEntry>
    {
        /// <summary>
        /// Determines whether the specified <see cref="PlaylistEntry"/> objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="PlaylistEntry"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="PlaylistEntry"/> to compare.</param>
        /// <returns>><b>true</b> if the specified <see cref="PlaylistEntry"/> objects are equal; otherwise, <b>false</b></returns>
        public bool Equals(PlaylistEntry x, PlaylistEntry y)
        {
            // Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y))
                return true;

            // Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            if (x.Id.Equals(y.Id) == false)
            {
                return false;
            }
            if (x.PlaylistId.Equals(y.PlaylistId) == false)
            {
                return false;
            }
            if (x.TrackId.Equals(y.TrackId) == false)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Returns a hash code for the specified <see cref="PlaylistEntry"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="PlaylistEntry"/> Object for which a hash code is to be returned</param>
        /// <returns>A hash code for the specified <see cref="PlaylistEntry"/> object.</returns>
        public int GetHashCode(PlaylistEntry obj)
        {
            if (obj != null)
            {
                return obj.Id.GetHashCode() ^ obj.PlaylistId.GetHashCode() ^ obj.TrackId.GetHashCode();
            }
            return 0;
        }
    }
}
