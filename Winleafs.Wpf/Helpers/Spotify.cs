using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Helpers
{
    public static class Spotify
    {
        private static SpotifyWebAPI _webAPI;

        public static SpotifyWebAPI WebAPI
        {
            get
            {
                if (_webAPI == null)
                {
                    _webAPI = new SpotifyWebAPI()
                    {
                        TokenType = UserSettings.Settings.SpotifyAPITokenType,
                        AccessToken = UserSettings.Settings.SpotifyAPIAccessToken
                    };
                }

                return _webAPI;
            }
        }

        public static void Connect(Action finishedCallback = null)
        {
            var auth = GetSpotifyAuthenticator(finishedCallback);
            
            auth.Start();
            auth.OpenBrowser();
        }

        public static async Task RefreshToken(Action finishedCallback = null)
        {
            var auth = GetSpotifyAuthenticator(finishedCallback);

            auth.Start();
            await auth.RefreshAuthAsync(UserSettings.Settings.SpotifyRefreshToken);
        }

        public static void Disconnect()
        {
            _webAPI = null;

            UserSettings.Settings.SpotifyAPITokenType = null;
            UserSettings.Settings.SpotifyAPIAccessToken = null;
            UserSettings.Settings.SpotifyRefreshToken = null;
            UserSettings.Settings.SaveSettings();
        }

        /// <summary>
        /// Retrieves the playlist that is currently playing.
        /// Returns null of the user is not palying any music.
        /// </summary>
        public static async Task<string> GetCurrentPlayingPlaylist()
        {
            var playbackContext = await WebAPI.GetPlaybackAsync();

            if (!playbackContext.IsPlaying)
            {
                return null;
            }

            var playlistUrl = playbackContext.Context.ExternalUrls.FirstOrDefault(externalUrl => externalUrl.Value.Contains("playlist"));
            var playlistId = playlistUrl.Value.Remove(0, "https://open.spotify.com/playlist/".Length);
            var playlist = await WebAPI.GetPlaylistAsync(playlistId);

            return playlist.Name;
        }

        /// <summary>
        /// Executes a request to get the playback state,
        /// returns true if that request is successful.
        /// </summary>
        public static bool WebAPIIsConnected()
        {
            try
            {
                var playbackContext = WebAPI.GetPlayback();
                return !playbackContext.HasError();
            }
            catch
            {
                return false;
            }
        }

        private static TokenSwapAuth GetSpotifyAuthenticator(Action finishedCallback = null)
        {
            var auth = new TokenSwapAuth(
                exchangeServerUri: "https://localhost:44330/api/spotify",
                serverUri: "http://localhost:4002",
               Scope.PlaylistReadPrivate | Scope.PlaylistReadCollaborative | Scope.UserReadCurrentlyPlaying | Scope.UserReadPlaybackState
            );

            auth.AuthReceived += async (sender, response) =>
            {
                var token = await auth.ExchangeCodeAsync(response.Code);

                UserSettings.Settings.SpotifyAPITokenType = token.TokenType;
                UserSettings.Settings.SpotifyAPIAccessToken = token.AccessToken;
                UserSettings.Settings.SpotifyRefreshToken = token.RefreshToken;
                UserSettings.Settings.SaveSettings();

                auth.Stop();

                _webAPI = null; //Reset the _webAPI object such that a new one is created at the next request with the new tokens

                finishedCallback?.Invoke();
            };

            auth.OnAccessTokenExpired += async (sender, e) =>
            {
                UserSettings.Settings.SpotifyAPIAccessToken = (await auth.RefreshAuthAsync(UserSettings.Settings.SpotifyRefreshToken)).AccessToken;
                UserSettings.Settings.SaveSettings();
            };

            return auth;
        }
    }
}
