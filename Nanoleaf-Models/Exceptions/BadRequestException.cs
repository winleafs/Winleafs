using System;

namespace Winleafs.Models.Exceptions
{
    /// <summary>
    /// An exception intended to be used to be thrown when the Nanoleaf device returns 400.
    /// </summary>
    public class BadRequestException : WinleafsException
    {
        /// <inheritdoc />
        public BadRequestException() : base()
        {
        }

        /// <inheritdoc />
        public BadRequestException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}