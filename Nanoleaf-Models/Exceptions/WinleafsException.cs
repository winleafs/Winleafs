using System;

namespace Winleafs.Models.Exceptions
{
    /// <summary>
    /// The overarching exception for Winleafs.
    /// Intended to be used when no other exception is available.
    /// </summary>
    [Serializable]
    public class WinleafsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WinleafsException"/> class.
        /// </summary>
        public WinleafsException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinleafsException"/> class.
        /// </summary>
        /// <param name="message">The message wanting to be shown.</param>
        public WinleafsException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinleafsException"/> class.
        /// </summary>
        /// <param name="message">The message wanting to be shown.</param>
        /// <param name="innerException">The inner exception that was thrown.</param>
        public WinleafsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}