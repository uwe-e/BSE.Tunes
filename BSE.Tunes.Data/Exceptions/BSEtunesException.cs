using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BSE.Tunes.Data.Exceptions
{
    /// <summary>
    /// Represents special errors that occur during BSEtunes application execution .
    /// </summary>
    public class BSEtunesException : Exception
    {
        /// <summary>
        /// Gets or sets the name of the executing assembly
        /// </summary>
        public virtual string AssemblyName
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().FullName; }
        }
        /// <summary>
        /// Initializes a new instance of the BSEtunesException class.
        /// </summary>
        public BSEtunesException() { }
        /// <summary>
        /// Initializes a new instance of the BSEtunesException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public BSEtunesException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the BSEtunesException class with a specified error message
        /// and a reference to the inner exception that is the cause of this BSEtunesException.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public BSEtunesException(string message, Exception inner) : base(message, inner) { }
    }
}
