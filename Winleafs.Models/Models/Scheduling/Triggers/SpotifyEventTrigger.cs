namespace Winleafs.Models.Models.Scheduling.Triggers
{
    /// <summary>
    /// Model class for a Spotify event trigger
    /// </summary>
    public class SpotifyEventTrigger : EventTrigger
    {
        public string PlaylistId { get; set; }

        /// <summary>
        /// Name of the playlist.
        /// Note: since this name is set when the playlist names are
        /// retrieved from Spotify, it can be outdated if the user changes
        /// the name in Spotify.
        /// </summary>
        public string PlaylistName { get; set; }

        public override string Description => PlaylistName;
    }
}
