using System;

namespace Winleafs.Nanoleaf
{
    public class NanoleafConnection
    {
        public Uri Uri { get; set; }

        public string Token { get; set; }

        public NanoleafConnection(Uri uri, string token)
        {
            Uri = uri;
            Token = token;
        }
    }
}
