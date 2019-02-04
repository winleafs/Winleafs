using System;
using System.Transactions;

namespace Winleafs.Models.Exceptions
{
    /// <summary>
    /// Intended to be thrown when the user is unauthorized to execute the action.
    /// </summary>
    public class UnauthorizedException : WinleafsException
    {
        /// <inheritdoc />
        public UnauthorizedException() : base()
        {    
        }

        /// <inheritdoc />
        public UnauthorizedException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}