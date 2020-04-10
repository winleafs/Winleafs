using System;
using Winleafs.Models.Models;
using Winleafs.Server.Endpoints;
using Winleafs.Server.Endpoints.Interfaces;

namespace Winleafs.Server
{
    public class WinleafsServerClient
    {
        internal Uri BaseUri = new Uri("https://localhost:44330/api");
        internal readonly string ApplicationIdParameter = $"applicationId={UserSettings.Settings.ApplicationId}";

        private ISpotifyEndpoint _spotifyEndpoint;

        public ISpotifyEndpoint SpotifyEndpoint => _spotifyEndpoint ?? (_spotifyEndpoint = new SpotifyEndpoint(this));
    }
}
