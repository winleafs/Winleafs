using System.Threading.Tasks;
using System.Timers;

namespace Winleafs.Orchestration.Events
{
    /// <summary>
    /// Event trigger that is activated when a Spotify playlist is played
    /// </summary>
    public class SpotifyEventTrigger : EventTriggerBase
    {
        private const int _timerMilliseconds = 10000; //10 seconds
        private readonly EventTriggersCollection _eventTriggersCollection;
        private readonly string _playlistId;
        private readonly Timer _playlistCheckTimer;

        public SpotifyEventTrigger(EventTriggersCollection eventTriggersCollection, Models.Scheduling.Triggers.SpotifyEventTrigger spotifyEventTrigger)
            : base(spotifyEventTrigger.Brightness, spotifyEventTrigger.EffectName, spotifyEventTrigger.Priority)
        {
            _eventTriggersCollection = eventTriggersCollection;
            _playlistId = spotifyEventTrigger.PlaylistId;

            _playlistCheckTimer = new Timer(_timerMilliseconds);
            _playlistCheckTimer.Elapsed += CheckProcess;
            _playlistCheckTimer.AutoReset = true;
            _playlistCheckTimer.Start();
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

        public override void Stop()
        {
            _playlistCheckTimer.Stop();
        }
    }
}
