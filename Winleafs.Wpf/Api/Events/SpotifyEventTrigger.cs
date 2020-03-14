using NLog;
using System;
using System.Threading.Tasks;
using System.Timers;
using Winleafs.Models.Enums;
using Winleafs.Models.Models.Scheduling.Triggers;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Api.Events
{
    /// <summary>
    /// Event trigger that is activated when a certain process starts
    /// </summary>
    public class SpotifyEventTrigger : IEventTrigger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ITrigger _trigger;
        private readonly Orchestrator _orchestrator;
        private readonly string _playlistName;
        private readonly string _effectName;
        private readonly int _brightness;
        private bool _isActive;

        public SpotifyEventTrigger(ITrigger trigger, Orchestrator orchestrator, string playlistName, string effectName, int brightness)
        {
            _trigger = trigger;
            _orchestrator = orchestrator;
            _playlistName = playlistName;
            _effectName = effectName;
            _brightness = brightness;
            _isActive = false;

            var processCheckTimer = new Timer(60000);
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
            var shouldBeActive = false;

            try
            {
                var currentPlaylistName = await Spotify.GetCurrentPlayingPlaylist();
                shouldBeActive = currentPlaylistName!= null && _playlistName.ToLower() == currentPlaylistName.ToLower();
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Retrieving the Spotify current playlist failed.");
            }

            if (shouldBeActive && !_isActive)
            {
                await TryStartEffect();
            }
            else
            {
                if (_isActive)
                {
                    //Let orchestrator know that the spotify event has stopped so it can continue with normal program, will not fail since an event can only be activated when no override is active
                    //Always return to schedule since only 1 event can be active at a time
                    await _orchestrator.TrySetOperationMode(OperationMode.Schedule);
                    _isActive = false;
                }
            }
        }

        /// <summary>
        /// Start the effect if possible
        /// </summary>
        private async Task TryStartEffect()
        {
            if (await _orchestrator.TrySetOperationMode(OperationMode.Event))
            {
                _isActive = true;
                await _orchestrator.ActivateEffect(_effectName, _brightness);
            }
        }

        public void StopEvent()
        {
            _isActive = false;
        }

        public bool IsActive()
        {
            return _isActive;
        }

        public ITrigger GetTrigger()
        {
            return _trigger;
        }
    }
}
