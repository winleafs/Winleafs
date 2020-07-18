namespace Winleafs.Wpf.ViewModels
{
    public class SpotifyPlaylistViewModel
    {
        public string PlaylistId { get; set; }

        public string PlaylistName { get; set; }

        public override string ToString()
        {
            return PlaylistName;
        }
    }
}
