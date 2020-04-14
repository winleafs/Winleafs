using NLog;
using System;
using System.Threading.Tasks;
using System.Timers;
using Winleafs.Models.Models;
using Winleafs.Server;

namespace Winleafs.Wpf.Api
{
    /// <summary>
    /// Static class used by Spotify events to check which playlist is active
    /// </summary>
    public static class SpotifyEventTimer
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public static string CurrentPlaylistId { get; private set; }

        private static Timer _timer;
        private static WinleafsServerClient _winleafsServerClient;

        static SpotifyEventTimer()
        {
            _timer = new Timer(60000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            _winleafsServerClient = new WinleafsServerClient();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Task.Run(() => GetCurrentPlaylistId());
        }

        private static async Task GetCurrentPlaylistId()
        {
            try
            {
                CurrentPlaylistId = await _winleafsServerClient.SpotifyEndpoint.GetCurrentPlayingPlaylistId();
            }
            catch (Exception ex)
            {
                CurrentPlaylistId = null;
                Logger.Warn(ex, "Error during retrieval of current playlist id");
            }
        }

        public static void Initialize()
        {
            if (UserSettings.Settings.ActiveSchedule != null && UserSettings.Settings.ActiveSchedule.HasSpotifyTriggers())
            {
                OnTimedEvent(null, null);
                _timer.Start();
            }
            else
            {
                _timer.Stop();
            }
        }
    }
}
