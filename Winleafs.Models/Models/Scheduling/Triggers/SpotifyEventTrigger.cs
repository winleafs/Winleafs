namespace Winleafs.Models.Models.Scheduling.Triggers
{
    /// <summary>
    /// Model class for a Spotify event trigger
    /// </summary>
    public class SpotifyEventTrigger : TriggerBase
    {
        public string PlaylistName { get; set; }

        public override string GetDescription()
        {
            return PlaylistName;
        }
    }
}
