using NLog;
using System.Threading.Tasks;
using System.Timers;

namespace Winleafs.Wpf.Api.Events
{
    /// <summary>
    /// Event trigger that is activated when a Spotify playlist is played
    /// </summary>
    public class SpotifyEventTrigger : EventTriggerBase
    {
        private const int _timerMilliseconds = 60000; //1 minute
        private readonly EventTriggersCollection _eventTriggersCollection;
        private readonly string _playlistId;

        public SpotifyEventTrigger(EventTriggersCollection eventTriggersCollection, Models.Models.Scheduling.Triggers.SpotifyEventTrigger spotifyEventTrigger)
            : base(spotifyEventTrigger.Brightness, spotifyEventTrigger.EffectName, spotifyEventTrigger.Priority)
        {
            _eventTriggersCollection = eventTriggersCollection;
            _playlistId = spotifyEventTrigger.PlaylistId;

            var processCheckTimer = new Timer(_timerMilliseconds);
            processCheckTimer.Elapsed += CheckProcess;
            processCheckTimer.AutoReset = true;
            processCheckTimer.Start();
        }

        private void CheckProcess(object source, ElapsedEventArgs e)
        {
            Task.Run(() => CheckPlaylistAsync());
        }

        /// <summary>
        /// Checks if a playlist is being played in Spotify then execute TryStartEffect(), else stop the effect
        /// </summary>
        private async Task CheckPlaylistAsync()
        {
            if (SpotifyEventTimer.CurrentPlaylistId == _playlistId)
            {
                await _eventTriggersCollection.ActivateTrigger(Priority);
            }
            else
            {
                await _eventTriggersCollection.DeactivateTrigger(Priority);
            }
        }
    }
}
