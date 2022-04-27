using System;
using Winleafs.Models;
using Winleafs.Server.Endpoints;
using Winleafs.Server.Endpoints.Interfaces;

namespace Winleafs.Server
{
    public class WinleafsServerClient
    {
        internal Uri BaseUri { get; set; }

        private ISpotifyEndpoint _spotifyEndpoint;

        public ISpotifyEndpoint SpotifyEndpoint => _spotifyEndpoint ?? (_spotifyEndpoint = new SpotifyEndpoint(this));

        public WinleafsServerClient()
        {
            BaseUri = new Uri(UserSettings.Settings.WinleafServerURL);
        }
    }
}
