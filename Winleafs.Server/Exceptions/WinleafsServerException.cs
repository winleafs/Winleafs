using System;

namespace Winleafs.Server.Exceptions
{
    public class WinleafsServerException : Exception
    {
        public WinleafsServerException(string message) : base(message)
        {

        }
    }
}
