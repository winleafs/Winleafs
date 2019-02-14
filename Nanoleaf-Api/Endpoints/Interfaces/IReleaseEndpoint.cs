using System.Threading.Tasks;

namespace Winleafs.Api.Endpoints.Interfaces
{
    public interface IReleaseEndpoint
    {
        /// <summary>
        /// Gets the latest version of the Winleafs application.
        /// </summary>
        /// <returns>The name of the latest release.</returns>
        Task<string> GetLatestVersion();
    }
}