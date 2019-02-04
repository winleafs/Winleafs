using System;

namespace Winleafs.Models.Exceptions
{
    /// <summary>
    /// Intended to be used when Nanoleaf returns a not found status code.
    /// </summary>
    public class NotFoundException : WinleafsException
    {
        /// <inheritdoc />
        public NotFoundException() : base()
        {
        }

        /// <inheritdoc />
        public NotFoundException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}