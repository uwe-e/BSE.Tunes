using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSE.Tunes.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an playlist with the same name exists.
    /// </summary>
    public class PlaylistExistsException : BSEtunesException
    {
        /// <summary>
        /// Initializes a new instance of the PlaylistExistsException class.
        /// </summary>
        public PlaylistExistsException() { }
        /// <summary>
        /// Initializes a new instance of the PlaylistExistsException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PlaylistExistsException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the PlaylistExistsException class with a specified error message
        /// and a reference to the inner exception that is the cause of this PlaylistExistsException.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public PlaylistExistsException(string message, Exception inner) : base(message, inner) { }
    }
}

