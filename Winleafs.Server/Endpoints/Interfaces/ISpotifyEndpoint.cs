using System.Collections.Generic;
using System.Threading.Tasks;

namespace Winleafs.Server.Endpoints.Interfaces
{
    public interface ISpotifyEndpoint
    {
        Dictionary<string, string> GetPlaylists();

        Task<string> GetCurrentPlayingPlaylistId();

        void Connect();
    }
}
