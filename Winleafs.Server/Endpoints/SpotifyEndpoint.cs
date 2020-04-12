using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Winleafs.Models.Models;
using Winleafs.Server.Endpoints.Interfaces;

namespace Winleafs.Server.Endpoints
{
    public class SpotifyEndpoint : WinleafsServerEndpoint, ISpotifyEndpoint
    {
        private const string BaseUrl = "spotify";

        public SpotifyEndpoint(WinleafsServerClient client)
        {
            Client = client;
        }

        public Dictionary<string, string> GetPlaylists()
        {
            return SendRequest<Dictionary<string, string>>($"{BaseUrl}/playlists/{UserSettings.Settings.ApplicationId}", Method.GET);
        }

        public Task<string> GetCurrentPlayingPlaylistId()
        {
            return SendRequestAsync<string>($"{BaseUrl}/current-playing-playlist-id/{UserSettings.Settings.ApplicationId}", Method.GET);
        }

        public void Disconnect()
        {
            SendRequest($"{BaseUrl}/disconnect/{UserSettings.Settings.ApplicationId}", Method.DELETE);
        }

        public bool IsConnected()
        {
            return SendRequest<bool>($"{BaseUrl}/is-connected/{UserSettings.Settings.ApplicationId}", Method.GET);
        }

        public void Connect()
        {
            //Open a browser window instead of sending a direct request. This is needed because the user needs a browser window to authenticate spotify
            Process.Start(new ProcessStartInfo("cmd", $"/c start {Client.BaseUri.OriginalString}/{BaseUrl}/authorize/{UserSettings.Settings.ApplicationId}") { CreateNoWindow = true });
        }
    }
}
